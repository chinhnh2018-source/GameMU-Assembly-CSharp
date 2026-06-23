using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class FriendsPart : UserControl
{
	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this.pages[0].btnRecommend, default(Vector4));
			}
			else if (id == 1000)
			{
				SystemHelpPart.SetMask(this.RecommendWin.btnSelectAll, default(Vector4));
			}
			else if (id == 1001)
			{
				SystemHelpPart.SetMask(this.RecommendWin.btnAddConfirm, default(Vector4));
			}
			else if (id == 1100)
			{
				SystemHelpPart.SetMask(this.RecommendWin.btnClose, default(Vector4));
			}
			else if (id == 10000)
			{
				SystemHelpPart.SetMask(this.btnClose, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	private void InitTextInPrefabs()
	{
		this.GTab.TabBtns[0].Text = Global.GetLang("好友");
		this.GTab.TabBtns[1].Text = Global.GetLang("仇人");
		this.GTab.TabBtns[2].Text = Global.GetLang("屏蔽");
		this.btnSearch.Text = Global.GetLang("添加");
		this.ConstUserName.Text = Global.GetLang("玩家名称");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.Root = this.Container;
		this.SubSearchWindow.SetActive(false);
		this.pages[0].PageType = 0;
		this.pages[0].SearchEventHandler = new DPSelectedItemEventHandler(this.SearchEventHandler);
		this.pages[0].FuMoEventHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			PlayZone.GlobalPlayZone.OpenFriendFuMoWindow();
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -10
				});
			}
		};
		this.pages[0].ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, e);
			}
		};
		this.pages[0].ShowConfirmWinCallback = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.DbID = e.ID;
			this.OperType = e.Flag;
			this.SubOperType = e.IDType;
			this.RoleName = e.Title;
			string message = string.Empty;
			if (e.Flag == 0)
			{
				message = Global.GetLang("确定要将该用户移出好友列表?");
			}
			else if (e.Flag == 1)
			{
				string text = string.Empty;
				switch (this.SubOperType)
				{
				case 0:
					text = Global.GetLang("好友");
					break;
				case 1:
					text = Global.GetLang("屏蔽");
					break;
				case 2:
					text = Global.GetLang("仇人");
					break;
				}
				message = Global.GetLang("对方已在您的好友列表中，是否移至") + text + Global.GetLang("列表?");
			}
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					if (this.OperType == 0)
					{
						GameInstance.Game.SpriteRemoveFriend(this.DbID);
					}
					else if (this.OperType == 1)
					{
						GameInstance.Game.SpriteAddFriend(this.DbID, this.RoleName, this.SubOperType);
					}
				}
			}, buttons);
		};
		this.pages[0].ShowRecommendWinCallback = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.RecommendWin = U3DUtils.NEW<FriendRecommendPart>();
			this.RecommendWin.transform.parent = base.transform;
			this.RecommendWin.transform.localPosition = new Vector3(0f, 0f, -2f);
			this.RecommendWin.transform.localScale = new Vector3(1f, 1f, 1f);
			this.RecommendWin.GetRecommendData();
			this.RecommendWin.DPSelectedItem = delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (this.RecommendWin != null)
				{
					this.RecommendWin.transform.parent = null;
					Object.Destroy(this.RecommendWin.gameObject);
					this.RecommendWin = null;
				}
				if (Global.Data.FriendRecommendDataList != null)
				{
					Global.Data.FriendRecommendDataList.Clear();
				}
				SystemHelpMgr.OnAction(UIObjIDs.FriendRecommendPart, HelpStateEvents.Inactived, 1);
			};
		};
		this.pages[1].PageType = 2;
		this.pages[1].SearchEventHandler = new DPSelectedItemEventHandler(this.SearchEventHandler);
		this.pages[1].ShowConfirmWinCallback = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.DbID = e.ID;
			this.OperType = e.Flag;
			this.SubOperType = e.IDType;
			this.RoleName = e.Title;
			string message = string.Empty;
			if (e.Flag == 0)
			{
				message = Global.GetLang("确定要将该用户移出仇人列表?");
			}
			else if (e.Flag == 1)
			{
				string text = string.Empty;
				switch (this.SubOperType)
				{
				case 0:
					text = Global.GetLang("好友");
					break;
				case 1:
					text = Global.GetLang("屏蔽");
					break;
				case 2:
					text = Global.GetLang("仇人");
					break;
				}
				message = Global.GetLang("对方已在您的仇人列表中，是否移至") + text + Global.GetLang("列表?");
			}
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					if (this.OperType == 0)
					{
						GameInstance.Game.SpriteRemoveFriend(this.DbID);
					}
					else if (this.OperType == 1)
					{
						GameInstance.Game.SpriteAddFriend(this.DbID, this.RoleName, this.SubOperType);
					}
				}
			}, buttons);
		};
		this.pages[2].PageType = 1;
		this.pages[2].SearchEventHandler = new DPSelectedItemEventHandler(this.SearchEventHandler);
		this.pages[2].ShowConfirmWinCallback = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.SubOperType = e.IDType;
			this.RoleName = e.Title;
			this.DbID = e.ID;
			this.OperType = e.Flag;
			string message = string.Empty;
			if (e.Flag == 0)
			{
				message = Global.GetLang("确定要将该用户移出屏蔽列表?");
			}
			else if (e.Flag == 1)
			{
				string empty = string.Empty;
				switch (this.SubOperType)
				{
				case 0:
					message = Global.GetLang("好友");
					break;
				case 1:
					message = Global.GetLang("屏蔽");
					break;
				case 2:
					message = Global.GetLang("仇人");
					break;
				}
				message = Global.GetLang("对方已在您的屏蔽列表中，是否移至") + empty + Global.GetLang("列表?");
			}
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					if (this.OperType == 0)
					{
						GameInstance.Game.SpriteRemoveFriend(this.DbID);
					}
					else if (this.OperType == 1)
					{
						GameInstance.Game.SpriteAddFriend(this.DbID, this.RoleName, this.SubOperType);
					}
				}
			}, buttons);
		};
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (ConfigVersionSystemOpen.IsVersionSystemOpen(100104) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.FuMo, ref num, ref num2, ref num3))
		{
			GameInstance.Game.SenUserFuMoRoleList();
			GameInstance.Game.SenFuMoMailList();
		}
		this.InitButtonListener();
	}

	private void SearchEventHandler(object sender, DPSelectedItemEventArgs e)
	{
		switch (e.IDType)
		{
		case 0:
			this.subWinTitle.spriteName = "tianjiaHaoyou";
			break;
		case 1:
			this.subWinTitle.spriteName = "tianjiaPingbi";
			break;
		case 2:
			this.subWinTitle.spriteName = "tianjiaChouren";
			break;
		}
		this.inputUserName.text = string.Empty;
		this.CurrentSelectedButton = e.IDType;
		this.SubSearchWindow.SetActive(true);
	}

	public override void Destroy()
	{
	}

	private void InitButtonListener()
	{
		this.btnSubWinClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SubSearchWindow.SetActive(false);
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -10
				});
			}
			SystemHelpMgr.OnAction(UIObjIDs.FriendsPart, HelpStateEvents.Inactived, 1);
		};
		this.btnSearch.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (null != this.inputUserName && this.inputUserName.text != string.Empty)
			{
				if (this.inputUserName.text == Global.FormatRoleName(Global.Data.roleData))
				{
					Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("不能添加自己为好友!"), new DPSelectedItemEventHandler(this.DPSelectItemHandler), new string[]
					{
						Global.GetLang("确定")
					});
				}
				else
				{
					FriendData friendData = Global.FindFriendData(this.inputUserName.text);
					if (friendData == null)
					{
						string text = this.inputUserName.text;
						Global.SendEvent("600", Global.GetLang("添加好友次数"));
						GameInstance.Game.SpriteAddFriend(-1, text, this.CurrentSelectedButton);
						this.inputUserName.text = string.Empty;
					}
					else if (friendData.FriendType != this.CurrentSelectedButton)
					{
						string otherRoleName = friendData.OtherRoleName;
						Global.SendEvent("600", Global.GetLang("添加好友次数"));
						GameInstance.Game.SpriteAddFriend(friendData.DbID, otherRoleName, this.CurrentSelectedButton);
					}
					else
					{
						string lang = Global.GetLang("好友");
						if (friendData.FriendType == 1)
						{
							lang = Global.GetLang("屏蔽");
						}
						else if (friendData.FriendType == 2)
						{
							lang = Global.GetLang("仇人");
						}
						Super.HintMainText(StringUtil.substitute(Global.GetLang("【{0}】已经在{1}列表中"), new object[]
						{
							this.inputUserName.text,
							lang
						}), 10, 3);
					}
				}
			}
		};
	}

	public void GetDataRefreshFuMoList(string str)
	{
		if (this.pages != null)
		{
			for (int i = 0; i < this.pages.Length; i++)
			{
				if (this.pages[i].PageType == 0)
				{
					this.pages[i].FuMoRoleList = str;
				}
			}
		}
		this.RefreshItemsList();
	}

	public void GetDataRefreshFuMoItem()
	{
		GameInstance.Game.SenUserFuMoRoleList();
	}

	public void InitPartData()
	{
		this.GetNewData();
	}

	public void ResetGetNewData()
	{
	}

	public void RefreshFuMoBtnTip(Dictionary<int, List<FuMoMailData>> data)
	{
		if (Global.Data.roleData != null && data != null && data.ContainsKey(Global.Data.roleData.RoleID))
		{
			if (data[Global.Data.roleData.RoleID] == null || data[Global.Data.roleData.RoleID].Count <= 0)
			{
				this.pages[0].GameFuMoTip.SetActive(false);
			}
			else
			{
				this.pages[0].GameFuMoTip.SetActive(true);
			}
		}
	}

	public void GetNewData()
	{
		GameInstance.Game.SpriteGetFriends();
		SystemHelpMgr.OnAction(UIObjIDs.FriendsPart, HelpStateEvents.Actived, 1);
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Root);
	}

	private string GetMapName(string position)
	{
		string[] array = position.Split(new char[]
		{
			':'
		});
		if (array == null)
		{
			return Global.GetLang("未知");
		}
		if (array.Length != 4)
		{
			return Global.GetLang("未知");
		}
		return ConfigSettings.GetMapNameByCode(Convert.ToInt32(array[0]), false);
	}

	public void RefreshItemsList()
	{
		if (this.SubSearchWindow.activeInHierarchy)
		{
			this.SubSearchWindow.SetActive(false);
		}
		for (int i = 0; i < this.pages.Length; i++)
		{
			if (this.pages[i].gameObject.activeInHierarchy)
			{
				this.pages[i].RefreshItemsListPerpage();
			}
		}
	}

	public object GetSelectedFrienData()
	{
		return this.friendListItem.Tag;
	}

	public void DPSelectItemHandler(object sender, DPSelectedItemEventArgs args)
	{
		this.inputUserName.text = string.Empty;
	}

	public void RefreshRecommendList()
	{
		if (this.RecommendWin != null && this.RecommendWin.gameObject.activeInHierarchy)
		{
			this.RecommendWin.RefreshCecommendList();
		}
	}

	private Canvas Root;

	public GButton btnClose;

	public GameObject SubSearchWindow;

	public GButton btnSearch;

	public GButton btnSubWinClose;

	public UIInput inputUserName;

	public UISprite subWinTitle;

	public FriendsPartPage[] pages;

	public FriendRecommendPart RecommendWin;

	public GTabControl GTab;

	public TextBlock ConstUserName;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int CurrentSelectedButton;

	private FriendListItem friendListItem;

	private int DbID = -1;

	private int SubOperType = -1;

	private string RoleName = string.Empty;

	private int OperType = -1;
}
