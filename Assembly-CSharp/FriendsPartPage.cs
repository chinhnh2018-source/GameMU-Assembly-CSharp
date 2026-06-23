using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class FriendsPartPage : UserControl
{
	private void InitTextInPrefabs()
	{
		if (this._pageType == 0)
		{
			this.btnAddItem.Text = Global.GetLang("添加好友");
			this.btnRecommend.Text = Global.GetLang("一键征友");
			this.btnFuMo.gameObject.SetActive(true);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (ConfigVersionSystemOpen.IsVersionSystemOpen(100104) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.FuMo, ref num, ref num2, ref num3))
			{
				this.btnFuMo.Text = Global.GetLang("领取奖励");
				this.btnFuMo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					if (this.SearchEventHandler != null)
					{
						this.FuMoEventHandler(this, new DPSelectedItemEventArgs());
					}
				};
			}
			else
			{
				this.btnFuMo.gameObject.SetActive(false);
			}
		}
		else if (this._pageType == 2)
		{
			this.btnAddItem.Text = Global.GetLang("添加仇人");
		}
		else if (this._pageType == 1)
		{
			this.btnAddItem.Text = Global.GetLang("添加屏蔽");
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.Root = this.Container;
		this.ItemCollection = this.listBox.ItemsSource;
		this.listBox.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.listBox_MouseLeftButtonUp);
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.btnAddItem.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.SearchEventHandler != null)
			{
				this.SearchEventHandler(this, new DPSelectedItemEventArgs
				{
					IDType = this.PageType
				});
			}
		};
		if (this.btnRecommend != null)
		{
			this.btnRecommend.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.ShowRecommendWinCallback != null)
				{
					this.ShowRecommendWinCallback(this, null);
				}
			};
		}
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

	private void listBox_MouseLeftButtonUp(object sender, MouseEvent e)
	{
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		if (null == this.listBox.LastSelectedItem && this.listBox.LastSelectedItem != this.listBox.SelectedItem)
		{
			FriendsPartPageItem friendsPartPageItem = U3DUtils.AS<FriendsPartPageItem>(this.listBox.SelectedItem);
			if (null != friendsPartPageItem)
			{
				friendsPartPageItem.SelectedState = true;
			}
			return;
		}
		if (null != this.listBox.LastSelectedItem && null != this.listBox.SelectedItem && this.listBox.LastSelectedItem != this.listBox.SelectedItem)
		{
			FriendsPartPageItem friendsPartPageItem = U3DUtils.AS<FriendsPartPageItem>(this.listBox.LastSelectedItem);
			if (null != friendsPartPageItem)
			{
				friendsPartPageItem.SelectedState = false;
			}
			friendsPartPageItem = U3DUtils.AS<FriendsPartPageItem>(this.listBox.SelectedItem);
			if (null != friendsPartPageItem)
			{
				friendsPartPageItem.SelectedState = true;
			}
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

	private GameObject CreatePageGroup()
	{
		GameObject gameObject = new GameObject();
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		boxCollider.size = new Vector3(760f, 240f, 0f);
		return gameObject;
	}

	private void AddItemToGroup(int groupIdx, List<FriendData> friendData)
	{
		GameObject gameObject = this.pages[groupIdx];
		int num = groupIdx * 5;
		while (num < groupIdx * 5 + 5 && num < friendData.Count)
		{
			num++;
		}
	}

	public string FuMoRoleList
	{
		get
		{
			return this.m_FuMoRoleList;
		}
		set
		{
			this.m_FuMoRoleList = value;
		}
	}

	public void RefreshItemsListPerpage()
	{
		this.ItemCollection.Clear();
		if (Global.Data.FriendDataList == null)
		{
			return;
		}
		List<FriendData> list = Global.Data.FriendDataList.FindAll((FriendData x) => x.FriendType == this.PageType);
		int num = (int)Mathf.Ceil((float)list.Count / 5f);
		for (int i = 0; i < num; i++)
		{
			FriendsPartPageItem friendsPartPageItem = U3DUtils.NEW<FriendsPartPageItem>();
			this.ItemCollection.AddNoUpdate(friendsPartPageItem);
			friendsPartPageItem.PageType = this.PageType;
			friendsPartPageItem.ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.ShowChatBoxCallback != null)
				{
					this.ShowChatBoxCallback(this, e);
				}
			};
			friendsPartPageItem.ShowConfirmWinCallback = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.ShowConfirmWinCallback != null)
				{
					this.ShowConfirmWinCallback(s, e);
				}
			};
			int num2 = (list.Count - i * 5 < 5) ? (list.Count - i * 5) : 5;
			friendsPartPageItem.AddItems(list.GetRange(i * 5, num2), this.FuMoRoleList);
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

	public void RefreshItemsList()
	{
		this.ItemCollection.Clear();
		if (Global.Data.FriendDataList == null)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < Global.Data.FriendDataList.Count; j++)
			{
				if (Global.Data.FriendDataList[j].FriendType == this.PageType)
				{
					FriendListItem friendListItem = U3DUtils.NEW<FriendListItem>();
					this.ItemCollection.AddNoUpdate(friendListItem);
					friendListItem.PageType = this.PageType;
					Global.Data.FriendDataList[j].OtherRoleName = Global.Data.FriendDataList[j].OtherRoleName;
					friendListItem.PersonImg = StringUtil.substitute("NetImages/Face/{0}0_0.png", new object[]
					{
						Global.CalcOriginalOccupationID(Global.Data.FriendDataList[j].Occupation)
					});
					friendListItem.RoleID = Global.Data.FriendDataList[j].OtherRoleID;
					friendListItem.DbID = Global.Data.FriendDataList[j].DbID;
					friendListItem.OtherRoleName = Global.Data.FriendDataList[j].OtherRoleName;
					friendListItem.OtherRoleLevel = string.Concat(new string[]
					{
						"LV",
						Global.Data.FriendDataList[j].OtherLevel.ToString(),
						"[",
						Global.Data.FriendDataList[j].FriendChangeLifeLev.ToString(),
						Global.GetLang("转]")
					});
					friendListItem.OtherRoleForce = Global.GetLang("战力：") + Global.Data.FriendDataList[j].FriendCombatForce;
					friendListItem.UserPosition = this.GetMapName(Global.Data.FriendDataList[j].Position);
					friendListItem.OnlineState = Global.Data.FriendDataList[j].OnlineState;
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
		}
		this.ItemCollection.DelayUpdate();
		if (this.ItemCollection.Count > 0)
		{
			this.listBox.SelectedIndex = 0;
		}
	}

	public void OnEnable()
	{
		this.RefreshItemsListPerpage();
	}

	private void SetRadioPos(int currentPage)
	{
		float x = ((float)currentPage - (float)(this.pageCount + 1) / 2f) * this.radioForeground.gameObject.transform.localScale.x;
		Vector3 localPosition = this.radioForeground.gameObject.transform.localPosition;
		localPosition.x = x;
		this.radioForeground.gameObject.transform.localPosition = localPosition;
	}

	private Canvas Root;

	public ListBox listBox;

	public GButton btnAddItem;

	public GButton btnRecommend;

	public GButton btnFuMo;

	public GameObject GameFuMoTip;

	public DPSelectedItemEventHandler FuMoEventHandler;

	public DPSelectedItemEventHandler SearchEventHandler;

	public DPSelectedItemEventHandler ShowChatBoxCallback;

	public DPSelectedItemEventHandler ShowConfirmWinCallback;

	public DPSelectedItemEventHandler ShowRecommendWinCallback;

	public UICenterOnChild centerController;

	public UIPanel panel;

	public GameObject radioGroup;

	public UISprite radioBackground;

	public UISprite radioForeground;

	private int pageCount;

	private ObservableCollection _ItemCollection;

	private int _pageType;

	private List<GameObject> pages = new List<GameObject>();

	private string m_FuMoRoleList = string.Empty;
}
