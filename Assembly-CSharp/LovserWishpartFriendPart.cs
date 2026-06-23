using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class LovserWishpartFriendPart : UserControl
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
			if (friendDataList[i].FriendType == 0)
			{
				LovserWishpartFriendPartItem lovserWishpartFriendPartItem = U3DUtils.NEW<LovserWishpartFriendPartItem>();
				FriendData friendData = friendDataList[i];
				lovserWishpartFriendPartItem.RoleID = friendData.OtherRoleID;
				lovserWishpartFriendPartItem.RoleName = friendData.OtherRoleName;
				this.ItemsSource.Add(lovserWishpartFriendPartItem);
				UIPanel component = lovserWishpartFriendPartItem.GetComponent<UIPanel>();
				if (component)
				{
					Object.Destroy(component);
				}
			}
		}
	}

	private void SelectedBtn(object sender, MouseEvent e)
	{
		LovserWishpartFriendPartItem lovserWishpartFriendPartItem = U3DUtils.AS<LovserWishpartFriendPartItem>(this.friendList.SelectedItem);
		if (lovserWishpartFriendPartItem == null)
		{
			return;
		}
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			ID = lovserWishpartFriendPartItem.RoleID,
			Title = lovserWishpartFriendPartItem.RoleName
		});
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox friendList;

	private ObservableCollection ItemsSource;
}
