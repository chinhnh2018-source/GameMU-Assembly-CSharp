using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class JingLingMapMini : UserControl
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

	private new void OnDestroy()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.TitleName.text = Global.GetLang("快速定位");
		if (this.bak)
		{
			this.bak.localScale = Super.GetScreenSize();
		}
		GameInstance.Game.GetJieriFanbeiInfo();
		this.ItemCollection = this.itemList.ItemsSource;
		this.RelationDataCount = 0;
		this.itemList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.FriendItemSelectedChange);
		if (this.editBak)
		{
			this.editBak.SetActive(false);
		}
		UIEventListener.Get(this.backBtn.gameObject).onClick = delegate(GameObject s)
		{
			JingLingMapEvent.ProcessEvent(EmJingMapEvent.LeaveJingLingMap);
		};
		UIEventListener.Get(this.cebianBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnCeBianClick);
		this.cebianBtn.tweenTarget.GetComponent<UISprite>().spriteName = "cehua1";
		this.leftHead.transform.localPosition = new Vector3(-290f, 0f, 0f);
		this.leftTopHead.transform.localPosition = new Vector3(-290f, 0f, 0f);
		this.leftDownHead.transform.localPosition = new Vector3(-290f, 0f, 0f);
		this.renwuRefreshBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnRenWuRefreshClick);
		this.nuliRefreshBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnNuLiRefreshClick);
		this.refreshBossBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnRefreshBossClick);
		for (int i = 0; i < this.m_Tab.TabBtns.Count; i++)
		{
			this.m_Tab.TabBtns[i].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this.m_BtnsText[i]
			}));
		}
		this.m_Tab.TabBtns[0].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			this.m_BtnsText[0]
		}));
		this.m_Tab.DPSelectedItem = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (s.ID == 0)
			{
				this.curPage = 0;
				this.RequestRelation();
			}
			else if (s.ID == 1)
			{
				if (Global.IsHavingBangHui())
				{
					this.curPage = 1;
					this.RequestRelation();
				}
				else
				{
					this.InitPage(1);
				}
			}
			for (int j = 0; j < this.m_Tab.TabBtns.Count; j++)
			{
				this.m_Tab.TabBtns[j].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					this.m_BtnsText[j]
				}));
				if (j == s.ID)
				{
					this.m_Tab.TabBtns[j].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"f0f0f0",
						this.m_BtnsText[j]
					}));
				}
			}
			return true;
		};
		this.curPage = 0;
		this.InitPage(this.curPage);
		long num = ConfigSystemParam.GetSystemParamIntByName("RefreshMissionCost");
		if (0L > num)
		{
			num = 0L;
		}
		this.renwuCostLbl.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			num.ToString()
		});
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "JingLingYaoSaiShuaXin", (int)num, string.Empty);
		GameInstance.Game.SpriteGetFriends();
		if (Global.IsHavingBangHui())
		{
			GameInstance.Game.SpriteGetBangHuiMemberDataList(Global.Data.roleData.Faction);
		}
		this.UpdateUI();
	}

	private void OnCeBianClick(GameObject go)
	{
		if (this.leftHead)
		{
			if (this.leftHead.transform.localPosition.x > -1f)
			{
				TweenPosition.Begin(this.leftHead, 0.2f, new Vector3(-290f, 0f, 0f));
				TweenPosition.Begin(this.leftTopHead, 0.2f, new Vector3(-290f, 0f, 0f));
				TweenPosition.Begin(this.leftDownHead, 0.2f, new Vector3(-290f, 0f, 0f));
				this.cebianBtn.tweenTarget.GetComponent<UISprite>().spriteName = "cehua1";
			}
			else
			{
				TweenPosition.Begin(this.leftHead, 0.2f, new Vector3(0f, 0f, 0f));
				TweenPosition.Begin(this.leftTopHead, 0.2f, new Vector3(0f, 0f, 0f));
				TweenPosition.Begin(this.leftDownHead, 0.2f, new Vector3(0f, 0f, 0f));
				this.cebianBtn.tweenTarget.GetComponent<UISprite>().spriteName = "cehua2";
			}
		}
	}

	private void OnRefreshBossClick(object sender, MouseEvent e)
	{
		if (JingLingMap.inst.bossface)
		{
			JingLingMap.inst.bossface.ClickFunction();
		}
	}

	public void RequestRelation()
	{
		if (this.curPage == 0)
		{
			GameInstance.Game.SpriteGetFriends();
		}
		else if (this.curPage == 1 && Global.IsHavingBangHui())
		{
			this.curPage = 1;
			GameInstance.Game.SpriteGetBangHuiMemberDataList(Global.Data.roleData.Faction);
		}
	}

	private void OnNuLiRefreshClick(object sender, MouseEvent e)
	{
		int num = ConfigSystemParam.GetSystemParamByName("ManorSearchCost", true).SafeToInt32(0);
		int num2 = Global.Data.roleData.Money1 + Global.Data.roleData.YinLiang;
		if (num2 < num)
		{
			Super.HintMainText(Global.GetLang("金币不足"), 10, 3);
			return;
		}
		GameInstance.Game.SendYaoSaiData(0);
		Super.ShowNetWaiting(null);
	}

	private void OnRenWuRefreshClick(object sender, MouseEvent e)
	{
		if (JingLingMap.IsInHome())
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(null, new DPSelectedItemEventArgs
				{
					IDType = 1
				});
			}
		}
		else
		{
			JingLingMap.inst.RequestChangeMap(Global.Data.roleData.RoleID);
			if (this.curSelectFriendItem != null)
			{
				this.curSelectFriendItem.selectSpr.gameObject.SetActive(false);
			}
			this.itemList.SelectedIndex = -1;
		}
	}

	private void FriendItemSelectedChange(object sender, MouseEvent e)
	{
		JingLingMapFriendItem curSelectFriendItem = this.curSelectFriendItem;
		if (null == curSelectFriendItem)
		{
			return;
		}
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			JingLingMapFriendItem component = this.ItemCollection.GetAt(i).GetComponent<JingLingMapFriendItem>();
			component.selectSpr.gameObject.SetActive(false);
		}
		curSelectFriendItem.OnItemClick();
	}

	public JingLingMapFriendItem curSelectFriendItem
	{
		get
		{
			JingLingMapFriendItem jingLingMapFriendItem = U3DUtils.AS<JingLingMapFriendItem>(this.itemList.SelectedItem);
			if (null == jingLingMapFriendItem)
			{
				return null;
			}
			return jingLingMapFriendItem;
		}
	}

	public void InitPage(int page)
	{
		this.curPage = page;
		this.InitList();
	}

	private void InitList()
	{
		this.ItemCollection.Clear();
		this.RePosition();
		if (this.curPage == 0)
		{
			if (Global.Data.FriendDataList == null)
			{
				return;
			}
			foreach (FriendData friendData in Global.Data.FriendDataList)
			{
				JingLingRelationData jingLingRelationData = new JingLingRelationData();
				jingLingRelationData.InitWith(friendData);
				if (friendData.FriendType == 0)
				{
					if (jingLingRelationData.IsOpenYaosai)
					{
						if (jingLingRelationData.RoleID != Global.Data.RoleID)
						{
							JingLingMapFriendItem jingLingMapFriendItem = U3DUtils.NEW<JingLingMapFriendItem>();
							jingLingMapFriendItem.ID = friendData.DbID;
							jingLingMapFriendItem.relationData.InitWith(friendData);
							jingLingMapFriendItem.UpdateUI();
							UIPanel component = jingLingMapFriendItem.GetComponent<UIPanel>();
							if (component)
							{
								Object.Destroy(component);
							}
							this.ItemCollection.AddNoUpdate(jingLingMapFriendItem);
						}
					}
				}
			}
		}
		else if (this.curPage == 1)
		{
			if (this.MyBangHuiMemberDataList == null)
			{
				return;
			}
			foreach (BangHuiMemberData bangHuiMemberData in this.MyBangHuiMemberDataList)
			{
				JingLingRelationData jingLingRelationData2 = new JingLingRelationData();
				jingLingRelationData2.InitWith(bangHuiMemberData);
				if (jingLingRelationData2.IsOpenYaosai)
				{
					if (jingLingRelationData2.RoleID != Global.Data.RoleID)
					{
						JingLingMapFriendItem jingLingMapFriendItem2 = U3DUtils.NEW<JingLingMapFriendItem>();
						jingLingMapFriendItem2.ID = bangHuiMemberData.RoleID;
						jingLingMapFriendItem2.relationData.InitWith(bangHuiMemberData);
						jingLingMapFriendItem2.UpdateUI();
						UIPanel component2 = jingLingMapFriendItem2.GetComponent<UIPanel>();
						if (component2)
						{
							Object.Destroy(component2);
						}
						this.ItemCollection.AddNoUpdate(jingLingMapFriendItem2);
					}
				}
			}
		}
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			JingLingMapFriendItem jingLingMapFriendItem3 = U3DUtils.AS<JingLingMapFriendItem>(this.ItemCollection[i]);
			if (jingLingMapFriendItem3.relationData.RoleID == JingLingMap.inst.curRoleID)
			{
				this.itemList.SelectedIndex = i;
				break;
			}
		}
	}

	private void RePosition()
	{
		this.PnlItem.transform.localPosition = new Vector3(-215f, 140f, -0.5f);
		this.PnlItem.transform.GetComponent<UIPanel>().clipRange = new Vector4(204f, -184f, 291f, 447f);
	}

	public virtual void UITimer_Tick(object sender, object e)
	{
		if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.MyHome && DateTime.MaxValue > JingLingMap.inst.FreeRefreshTime)
		{
			if (0L <= Global.GetCorrectDateTime().Ticks - JingLingMap.inst.FreeRefreshTime.Ticks)
			{
				this.renwuRefreshBtn.Text = Global.GetLang("刷新任务");
				this.renwuKeLingQuLbl.gameObject.SetActive(true);
				NGUITools.SetActive(this.renwuCDLbl.transform.parent.gameObject, false);
				this.renwuCostLbl.text = Global.GetLang("免费");
			}
			else
			{
				TimeSpan timeSpan;
				timeSpan..ctor(JingLingMap.inst.FreeRefreshTime.Ticks - Global.GetCorrectDateTime().Ticks);
				this.renwuRefreshBtn.Text = Global.GetLang("刷新任务");
				this.renwuKeLingQuLbl.gameObject.SetActive(true);
				string text = string.Empty;
				text += ((10 <= Mathf.FloorToInt((float)timeSpan.TotalHours)) ? Mathf.FloorToInt((float)timeSpan.TotalHours).ToString() : ("0" + Mathf.FloorToInt((float)timeSpan.TotalHours)));
				text = text + ":" + ((10 <= Mathf.FloorToInt((float)timeSpan.Minutes)) ? Mathf.FloorToInt((float)timeSpan.Minutes).ToString() : ("0" + Mathf.FloorToInt((float)timeSpan.Minutes)));
				text = text + ":" + ((10 <= Mathf.FloorToInt((float)timeSpan.Seconds)) ? Mathf.FloorToInt((float)timeSpan.Seconds).ToString() : ("0" + Mathf.FloorToInt((float)timeSpan.Seconds)));
				text += Global.GetLang("后免费");
				this.renwuCDLbl.text = text;
				NGUITools.SetActive(this.renwuCDLbl.transform.parent.gameObject, true);
				long num = ConfigSystemParam.GetSystemParamIntByName("RefreshMissionCost");
				if (0L > num)
				{
					num = 0L;
				}
				this.renwuCostLbl.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					num.ToString()
				});
				IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "JingLingYaoSaiShuaXin", (int)num, string.Empty);
			}
		}
		if (Global.IsInJingLingYaoSai() && JingLingMap.inst.mapmini != null && JingLingMap.inst.mapminiwindow != null)
		{
			JingLingMap.inst.mapmini.gameObject.SetActive(true);
			JingLingMap.inst.mapminiwindow.gameObject.SetActive(true);
		}
	}

	public void RefreshItemsList(List<BangHuiMemberData> bangHuiMemberDataList = null)
	{
		this.RelationDataCount++;
		if (bangHuiMemberDataList != null)
		{
			this.MyBangHuiMemberDataList = bangHuiMemberDataList;
		}
		if (this.RelationDataCount == 1)
		{
		}
		if (this.RelationDataCount == 2)
		{
			int num = 0;
			if (Global.Data.FriendDataList != null)
			{
				foreach (FriendData friendData in Global.Data.FriendDataList)
				{
					JingLingRelationData jingLingRelationData = new JingLingRelationData();
					jingLingRelationData.InitWith(friendData);
					if (friendData.FriendType == 0)
					{
						if (jingLingRelationData.IsOpenYaosai)
						{
							if (jingLingRelationData.RoleID != Global.Data.RoleID)
							{
								num++;
							}
						}
					}
				}
			}
			int num2 = 0;
			if (this.MyBangHuiMemberDataList != null)
			{
				foreach (BangHuiMemberData data in this.MyBangHuiMemberDataList)
				{
					JingLingRelationData jingLingRelationData2 = new JingLingRelationData();
					jingLingRelationData2.InitWith(data);
					if (jingLingRelationData2.IsOpenYaosai)
					{
						if (jingLingRelationData2.RoleID != Global.Data.RoleID)
						{
							num2++;
						}
					}
				}
			}
			if (num == 0 && Global.IsHavingBangHui() && num2 > 0)
			{
				this.curPage = 1;
				this.InitPage(this.curPage);
				for (int i = 0; i < this.m_Tab.TabBtns.Count; i++)
				{
					this.m_Tab.TabBtns[i].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						this.m_BtnsText[i]
					}));
					if (i == this.curPage)
					{
						this.m_Tab.TabBtns[i].Text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
						{
							"f0f0f0",
							this.m_BtnsText[i]
						}));
					}
				}
			}
			else
			{
				this.curPage = 0;
				this.InitList();
			}
			if (num > 0 || num2 > 0)
			{
				this.OnCeBianClick(null);
			}
		}
		else
		{
			this.InitList();
		}
	}

	public void UpdateUI()
	{
		if (PlayZone.GlobalPlayZone.mJingLingMapMini == null)
		{
			return;
		}
		long num = ConfigSystemParam.GetSystemParamIntByName("RefreshMissionCost");
		if (0L > num)
		{
			num = 0L;
		}
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "JingLingYaoSaiShuaXin", (int)num, string.Empty);
		if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.MyHome)
		{
			this.anchLeft.gameObject.SetActive(true);
			this.anchLeftTop.gameObject.SetActive(true);
			this.nuliRoot.gameObject.SetActive(false);
			this.renwuRoot.gameObject.SetActive(true);
			this.renwuKeLingQuLbl.gameObject.SetActive(true);
			if (!this.renwuCDRoot.gameObject.activeSelf)
			{
				this.renwuCDRoot.gameObject.SetActive(true);
			}
			if (DateTime.MinValue < JingLingMap.inst.FreeRefreshTime && DateTime.MaxValue > JingLingMap.inst.FreeRefreshTime)
			{
				if (0L <= Global.GetCorrectDateTime().Ticks - JingLingMap.inst.FreeRefreshTime.Ticks)
				{
					this.renwuRefreshBtn.Text = Global.GetLang("免费刷新");
					this.renwuKeLingQuLbl.gameObject.SetActive(true);
				}
			}
			else
			{
				this.renwuRefreshBtn.Text = Global.GetLang("刷新任务");
				this.renwuKeLingQuLbl.gameObject.SetActive(true);
			}
			this.UITimer_Tick(null, null);
		}
		else if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.FriendHome)
		{
			this.anchLeft.gameObject.SetActive(true);
			this.anchLeftTop.gameObject.SetActive(false);
			if (this.renwuCDRoot.gameObject.activeSelf)
			{
				this.renwuCDRoot.gameObject.SetActive(false);
			}
			this.renwuRefreshBtn.Text = Global.GetLang("回家");
			this.renwuKeLingQuLbl.gameObject.SetActive(false);
			if (JingLingMap.inst.mapmini.curSelectFriendItem != null)
			{
			}
		}
		else if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.NuLiSearch)
		{
			this.anchLeft.gameObject.SetActive(false);
			this.anchLeftTop.gameObject.SetActive(false);
			this.nuliRoot.gameObject.SetActive(true);
			this.renwuRoot.gameObject.SetActive(false);
			this.renwuKeLingQuLbl.gameObject.SetActive(false);
			this.nuliRefreshBtn.Text = Global.GetLang("换一个");
			this.nuliCostLbl.text = ConfigSystemParam.GetSystemParamByName("ManorSearchCost", true);
		}
		else
		{
			this.anchLeft.gameObject.SetActive(false);
			this.anchLeftTop.gameObject.SetActive(false);
			this.nuliRoot.gameObject.SetActive(false);
			this.renwuRoot.gameObject.SetActive(false);
			this.renwuKeLingQuLbl.gameObject.SetActive(false);
		}
	}

	public new void Update()
	{
		LeaderInfo.LeaderPos = this.rolePos;
		this.updateCD += Time.deltaTime;
		if (this.updateCD < 0.5f)
		{
			return;
		}
		this.updateCD = 0f;
		this.updateCDCout++;
		if (JingLingMap.pz.GameHintQueueIcon != null && JingLingMap.pz.GameHintQueueIcon.gameObject.activeSelf)
		{
			JingLingMap.pz.GameHintQueueIcon.gameObject.SetActive(false);
		}
		for (int i = 0; i < JingLingMap.pz.transform.childCount; i++)
		{
			Transform child = JingLingMap.pz.transform.GetChild(i);
			if (child.name.StartsWith("TeamGroupBox(Clone)") || child.name.StartsWith("BuffBoxMini(Clone)") || child.name.StartsWith("TaskBoxMini(Clone)") || child.name.StartsWith("AutoFindDeco(Clone)") || child.name.StartsWith("AutoFightDeco(Clone)") || child.name.StartsWith("ZhanMengHongBaoTipIcon(Clone)") || child.name.StartsWith("SystemHongBaoTipIcon(Clone)") || child.name.StartsWith("ActivityHint(Clone)") || child.name.StartsWith("MingXiangAwardBar(Clone)") || child.name.StartsWith("HintQueueIcon(Clone)"))
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	internal void OnBossDataChange(YaoSaiBossData bossdata)
	{
		if (bossdata == null)
		{
			return;
		}
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"boss data change owerid ",
				bossdata.OwnerID,
				" bossid ",
				bossdata.BossID,
				" life ",
				bossdata.LifeV
			})
		});
		JingLingRelationData jingLingRelationData = null;
		if (this.curPage == 0)
		{
			if (Global.Data.FriendDataList == null)
			{
				return;
			}
			foreach (FriendData friendData in Global.Data.FriendDataList)
			{
				JingLingRelationData jingLingRelationData2 = new JingLingRelationData();
				jingLingRelationData2.InitWith(friendData);
				if (friendData.FriendType == 0)
				{
					if (jingLingRelationData2.IsOpenYaosai)
					{
						if (jingLingRelationData2.RoleID != Global.Data.RoleID)
						{
							if (jingLingRelationData2.RoleID == bossdata.OwnerID)
							{
								jingLingRelationData = jingLingRelationData2;
								break;
							}
						}
					}
				}
			}
		}
		else if (this.curPage == 1)
		{
			if (this.MyBangHuiMemberDataList == null)
			{
				return;
			}
			foreach (BangHuiMemberData data in this.MyBangHuiMemberDataList)
			{
				JingLingRelationData jingLingRelationData3 = new JingLingRelationData();
				jingLingRelationData3.InitWith(data);
				if (jingLingRelationData3.IsOpenYaosai)
				{
					if (jingLingRelationData3.RoleID != Global.Data.RoleID)
					{
						if (jingLingRelationData3.RoleID == bossdata.OwnerID)
						{
							jingLingRelationData = jingLingRelationData3;
							break;
						}
					}
				}
			}
		}
		if (jingLingRelationData != null)
		{
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				JingLingMapFriendItem jingLingMapFriendItem = U3DUtils.AS<JingLingMapFriendItem>(this.ItemCollection[i]);
				if (jingLingMapFriendItem.relationData.RoleID == jingLingRelationData.RoleID)
				{
					jingLingMapFriendItem.OnBossDataChange(bossdata);
				}
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GameObject anchLeft;

	public GameObject anchLeftTop;

	public GameObject editBak;

	public UIButton backBtn;

	public ListBox itemList;

	public GameObject PnlItem;

	public GButton[] m_Btns;

	public GTab m_Tab;

	public GameObject leftHead;

	public GameObject leftTopHead;

	public GameObject leftDownHead;

	public UIButton cebianBtn;

	public GameObject renwuRoot;

	public GameObject nuliRoot;

	public GameObject renwuCDRoot;

	public GButton renwuRefreshBtn;

	public UILabel renwuCDLbl;

	public UILabel renwuCostLbl;

	public UILabel renwuKeLingQuLbl;

	public GButton refreshBossBtn;

	public Transform bak;

	public UILabel TitleName;

	public GButton nuliRefreshBtn;

	public UILabel nuliCostLbl;

	public List<UISprite> listDaiBi = new List<UISprite>();

	public Vector3 rolePos = new Vector3(27.5f, 50.2f, 25.4f);

	private ObservableCollection _ItemCollection;

	private string[] m_BtnsText = new string[]
	{
		Global.GetLang("好友"),
		Global.GetLang("战盟")
	};

	private int curPage;

	private List<BangHuiMemberData> MyBangHuiMemberDataList;

	public int RelationDataCount;

	private float updateCD;

	private int updateCDCout;
}
