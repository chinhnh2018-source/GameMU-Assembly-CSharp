using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class FriendRecommendPart : UserControl
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

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		if (null != this.listBox.SelectedItem)
		{
			FriendRecommendItem friendRecommendItem = U3DUtils.AS<FriendRecommendItem>(this.listBox.SelectedItem);
			bool selectedState = friendRecommendItem.SelectedState;
			friendRecommendItem.SelectedState = !selectedState;
		}
	}

	private void InitTextInPrefabs()
	{
		this.btnAddConfirm.Text = Global.GetLang("添加");
		this.btnSelectAll.Text = Global.GetLang("全选");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.listBox.ItemsSource;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.btnSelectAll.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				bool selectedState = U3DUtils.AS<FriendRecommendItem>(this.ItemCollection[i]).SelectedState;
				if (!selectedState)
				{
					U3DUtils.AS<FriendRecommendItem>(this.ItemCollection[i]).SelectedState = !selectedState;
				}
			}
			SystemHelpMgr.OnAction(UIObjIDs.FriendRecommendPartBtnAll, HelpStateEvents.Clicked, 1);
		};
		this.btnAddConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string text = string.Empty;
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				FriendRecommendItem friendRecommendItem = U3DUtils.AS<FriendRecommendItem>(this.ItemCollection[i]);
				if (friendRecommendItem.chkSelected.Check)
				{
					text = text + friendRecommendItem.RoleID + ",";
				}
			}
			if (text.Length > 0)
			{
				text = text.Substring(0, text.Length - 1);
				Global.SendEvent("601", Global.GetLang("一键征友次数"));
				GameInstance.Game.SpriteOneKeyAddFriendCmd(text);
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, null);
				}
			}
			SystemHelpMgr.OnAction(UIObjIDs.FriendRecommendPartBtnAdd, HelpStateEvents.Clicked, 1);
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, null);
			}
		};
	}

	public void GetRecommendData()
	{
		GameInstance.Game.SpriteOneKeyFindFriendCmd();
		SystemHelpMgr.OnAction(UIObjIDs.FriendRecommendPart, HelpStateEvents.Actived, 1);
	}

	public void RefreshCecommendList()
	{
		this.ItemCollection.Clear();
		foreach (OneKeyFindFriendData oneKeyFindFriendData in Global.Data.FriendRecommendDataList)
		{
			FriendRecommendItem friendRecommendItem = U3DUtils.NEW<FriendRecommendItem>();
			this.ItemCollection.AddNoUpdate(friendRecommendItem);
			friendRecommendItem.RoleID = oneKeyFindFriendData.m_nRoleID;
			friendRecommendItem.RoleName = oneKeyFindFriendData.m_nRoleName;
			friendRecommendItem.Occupation = Global.GetOccupationStr(oneKeyFindFriendData.m_nOccupation);
			friendRecommendItem.RoleLevel = string.Concat(new object[]
			{
				oneKeyFindFriendData.m_nLevel,
				Global.GetLang("级["),
				oneKeyFindFriendData.m_nChangeLifeLev,
				Global.GetLang("转]")
			});
			UIPanel component = friendRecommendItem.transform.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	public GButton btnAddConfirm;

	public GButton btnSelectAll;

	public GButton btnClose;

	public ListBox listBox;

	private ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;
}
