using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class FriendsPartPageItem : UserControl
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
		this.lblDbgInfo.text = string.Concat(new object[]
		{
			"scale:[",
			base.scaleX,
			":",
			base.scaleY,
			"]"
		});
	}

	public int PageType
	{
		get
		{
			return this._pageType;
		}
		set
		{
			this._pageType = value;
		}
	}

	public void AddItems(List<FriendData> friendDataList, string FuMoRoleList)
	{
		this.itemCount = friendDataList.Count;
		this.ItemCollection.Clear();
		string[] array = FuMoRoleList.Split(new char[]
		{
			'_'
		});
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		bool flag = false;
		if (ConfigVersionSystemOpen.IsVersionSystemOpen(100104) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.FuMo, ref num, ref num2, ref num3))
		{
			flag = true;
		}
		for (int i = 0; i < friendDataList.Count; i++)
		{
			FriendListItem friendListItem = U3DUtils.NEW<FriendListItem>();
			this.ItemCollection.AddNoUpdate(friendListItem);
			friendListItem.PageType = this.PageType;
			friendListItem.PersonImg = StringUtil.substitute("NetImages/Face/{0}0_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(friendDataList[i].Occupation)
			});
			friendListItem.RoleID = friendDataList[i].OtherRoleID;
			friendListItem.DbID = friendDataList[i].DbID;
			friendListItem.OtherRoleName = friendDataList[i].OtherRoleName;
			friendListItem.OtherRoleLevel = string.Concat(new string[]
			{
				"LV",
				friendDataList[i].OtherLevel.ToString(),
				"[",
				friendDataList[i].FriendChangeLifeLev.ToString(),
				Global.GetLang("转]")
			});
			friendListItem.OtherRoleForce = Global.GetLang("战力：") + friendDataList[i].FriendCombatForce;
			friendListItem.UserPosition = this.GetMapName(friendDataList[i].Position);
			friendListItem.OnlineState = friendDataList[i].OnlineState;
			if (this.PageType == 0)
			{
				if (flag)
				{
					friendListItem.btnFuMoZengSong.gameObject.SetActive(true);
					for (int j = 0; j < array.Length; j++)
					{
						if (string.IsNullOrEmpty(array[j]))
						{
							friendListItem.SetFuMo = true;
							break;
						}
						if (array[j].SafeToInt32(0) == friendListItem.RoleID)
						{
							friendListItem.SetFuMo = false;
							break;
						}
						if (j >= array.Length - 1)
						{
							friendListItem.SetFuMo = true;
						}
					}
				}
				else
				{
					friendListItem.btnFuMoZengSong.gameObject.SetActive(false);
				}
			}
			friendListItem.ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.ShowChatBoxCallback != null)
				{
					this.ShowChatBoxCallback(this, e);
				}
			};
			friendListItem.ShowConfirmWinCallback = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.ShowConfirmWinCallback != null)
				{
					this.ShowConfirmWinCallback(s, e);
				}
			};
		}
	}

	public void OnClick()
	{
		int num = this.CalculateClickArea();
		if (num != -1)
		{
			FriendListItem selectedItem = U3DUtils.AS<FriendListItem>(this.ItemCollection[num]);
			this.UnSelectedItem = this.SelectedItem;
			this.SelectedItem = selectedItem;
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
		if (UICamera.lastTouchPosition.y / Global.Data.ScreenScaleY < 342f && UICamera.lastTouchPosition.y / Global.Data.ScreenScaleY > 109f)
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
			return (num >= this.itemCount) ? -1 : num;
		}
		return -1;
	}

	public DPSelectedItemEventHandler ShowChatBoxCallback;

	public DPSelectedItemEventHandler ShowConfirmWinCallback;

	private FriendListItem UnSelectedItem;

	private FriendListItem SelectedItem;

	public UILabel lblDbgInfo;

	public ListBox listBox;

	private ObservableCollection _ItemCollection;

	private int _pageType;

	private int itemCount = -1;

	private bool _SelectedState;
}
