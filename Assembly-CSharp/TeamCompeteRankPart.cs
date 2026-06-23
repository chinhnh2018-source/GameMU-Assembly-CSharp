using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeamCompeteRankPart : UserControl, IMUEventManagerHandler
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

	public ObservableCollection TeamMembersItemCollection
	{
		get
		{
			return this._TeamMembersItemCollection;
		}
		set
		{
			this._TeamMembersItemCollection = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.TeamMembersItemCollection = this.mTeamMembersListBox.Items;
		this.mTeamMembersListBox.SelectionChanged = delegate(object s, MouseEvent e)
		{
			GameObject selectedItem = this.mTeamMembersListBox.SelectedItem;
			if (selectedItem == null)
			{
				return;
			}
			TeamCompeteRankTeamInfo component = selectedItem.GetComponent<TeamCompeteRankTeamInfo>();
			if (component == null)
			{
				return;
			}
			if (this.lastRoleInfoId == component.roleData.RoleID)
			{
				return;
			}
			this.RepositionTeamMembersItem(component);
			this.RefreshZhanDuiPlayerRoleInfo(component.zhanDuiData, component.roleData);
		};
		this.ItemCollection = this.mListBox.Items;
		this.mListBox.SelectionChanged = delegate(object s, MouseEvent e)
		{
			if (this.mLastTeamCompeteRankItem != null)
			{
				this.mLastTeamCompeteRankItem.IsSelect = false;
			}
			GameObject selectedItem = this.mListBox.SelectedItem;
			if (selectedItem == null)
			{
				return;
			}
			TeamCompeteRankItem component = selectedItem.GetComponent<TeamCompeteRankItem>();
			if (component == null)
			{
				return;
			}
			this.mLastTeamCompeteRankItem = component;
			component.IsSelect = true;
			if (component.zhanDuiData != null)
			{
				if (this.lastTeamId == component.zhanDuiData.ZhanDuiID)
				{
					return;
				}
				this.lastTeamId = component.zhanDuiData.ZhanDuiID;
			}
			this.RefreshZhanDuiInfo(component.zhanDuiData, component.RankId);
			this.ShowTeamMembersInfo(component.zhanDuiData);
		};
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.LblRank.Label.text = Global.GetLang("0");
		this.LblDuanWei.Label.text = Global.GetLang("暂无");
		this.LblScore.Label.text = Global.GetLang("0");
		this.LblTName.Label.text = Global.GetLang(string.Empty);
		this.LblServerName.Label.text = Global.GetLang(string.Empty);
		this.LblZhanLiInfo.Label.text = Global.GetLang(string.Empty);
		this.LblDuanWeiInfo.Label.text = Global.GetLang(string.Empty);
		this.LblScoreInfo.Label.text = Global.GetLang(string.Empty);
		this.LblRank.FontSize = 15;
		this.LblDuanWei.FontSize = 15;
		this.LblDuanWei.Y = -1.0;
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, null);
			}
		};
	}

	private void InitValue()
	{
		this.InitSelfInfo(TeamCompeteDataManager.MainZhanDuiData);
		this.RequestRankData();
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_QUERY_DAY_PAIHANG", new Action<MUSocketConnectEventArgs>(this.RespondRankData));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_QUERY_DAY_PAIHANG", new Action<MUSocketConnectEventArgs>(this.RespondRankData));
	}

	public void RequestRankData()
	{
		GameInstance.Game.RequestDuanWeiRankInfoMsg();
	}

	public void RespondRankData(MUSocketConnectEventArgs e)
	{
		List<TianTi5v5ZhanDuiData> list = DataHelper.BytesToObject<List<TianTi5v5ZhanDuiData>>(e.bytesData, 0, e.bytesData.Length);
		if (list != null)
		{
			this.LoadItems(list);
		}
	}

	private void InitSelfInfo(TianTi5v5ZhanDuiData data)
	{
		if (data == null)
		{
			return;
		}
		this.LblRank.Text = this.RankInfo(data.DuanWeiRank);
		this.LblDuanWei.Text = TeamCompeteDataManager.GetDuanWeiNameByID(data.DuanWeiId);
		this.LblScore.Text = data.DuanWeiJiFen.ToString();
	}

	private string RankInfo(int rankId)
	{
		if (rankId <= 100)
		{
			return rankId.ToString();
		}
		return Global.GetLang("100名以后");
	}

	private void LoadItems(List<TianTi5v5ZhanDuiData> datas)
	{
		if (datas == null || datas.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < datas.Count; i++)
		{
			TeamCompeteRankItem teamCompeteRankItem = U3DUtils.NEW<TeamCompeteRankItem>();
			teamCompeteRankItem.InitValue(i + 1, datas[i]);
			NGUITools.AddChild2(this.mListBox.gameObject, teamCompeteRankItem);
			this.ItemCollection.Add(teamCompeteRankItem);
			UIPanel component = teamCompeteRankItem.gameObject.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
		this.mListBox.SelectedIndex = 0;
	}

	private void ShowTeamMembersInfo(TianTi5v5ZhanDuiData data)
	{
		if (data == null || data.teamerList == null || data.teamerList.Count <= 0)
		{
			return;
		}
		if (this.TeamMembersItemCollection.Count > 0)
		{
			this.TeamMembersItemCollection.Clear();
			this.left.Clear();
			this.right.Clear();
			this.selectItemIndex = -1;
			this.secondSelectItemIndex = -1;
		}
		List<TianTi5v5ZhanDuiRoleData> teamerList = data.teamerList;
		this.SortByTeamleader(data.LeaderRoleID, teamerList);
		for (int i = 0; i < teamerList.Count; i++)
		{
			TeamCompeteRankTeamInfo teamCompeteRankTeamInfo = U3DUtils.NEW<TeamCompeteRankTeamInfo>();
			teamCompeteRankTeamInfo.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
			NGUITools.AddChild2(this.mTeamMembersListBox.gameObject, teamCompeteRankTeamInfo.gameObject);
			teamCompeteRankTeamInfo.InitValue(data, teamerList[i]);
			this.TeamMembersItemCollection.Add(teamCompeteRankTeamInfo);
		}
		this.mTeamMembersListBox.repositionNow = true;
		if (this.TeamMembersItemCollection.Count > 0)
		{
			this.mTeamMembersListBox.SelectedIndex = 0;
		}
	}

	private void RepositionTeamMembersItem(TeamCompeteRankTeamInfo item)
	{
		this.RepositionTeamMembersItemByPosition(item);
	}

	private void RepositionTeamMembersItemByPosition(TeamCompeteRankTeamInfo item)
	{
		if (this.left.Count > 0 || this.right.Count > 0)
		{
			for (int i = 0; i < this.TeamMembersItemCollection.Count; i++)
			{
				TeamCompeteRankTeamInfo component = this.TeamMembersItemCollection.GetAt(i).GetComponent<TeamCompeteRankTeamInfo>();
				if (component.roleData.RoleID == item.roleData.RoleID)
				{
					this.secondSelectItemIndex = i;
					break;
				}
			}
			if (this.selectItemIndex == this.secondSelectItemIndex)
			{
				return;
			}
		}
		if (this.left != null && this.left.Count > 0)
		{
			if (this.left != null && this.left.Count > 0)
			{
				for (int j = 0; j < this.left.Count; j++)
				{
					this.left[j].transform.localPosition = new Vector3(this.left[j].transform.localPosition.x + (float)this.offset, this.left[j].transform.localPosition.y, this.left[j].transform.localPosition.z);
					this.left[j].transform.localScale = this.localSacle;
				}
			}
		}
		if (this.right != null && this.right.Count > 0)
		{
			if (this.right != null && this.right.Count > 0)
			{
				for (int k = 0; k < this.right.Count; k++)
				{
					this.right[k].transform.localPosition = new Vector3(this.right[k].transform.localPosition.x - (float)this.offset, this.right[k].transform.localPosition.y, this.right[k].transform.localPosition.z);
					this.right[k].transform.localScale = this.localSacle;
				}
			}
		}
		this.left.Clear();
		this.right.Clear();
		this.selectItemIndex = -1;
		this.secondSelectItemIndex = -1;
		for (int l = 0; l < this.TeamMembersItemCollection.Count; l++)
		{
			TeamCompeteRankTeamInfo component2 = this.TeamMembersItemCollection.GetAt(l).GetComponent<TeamCompeteRankTeamInfo>();
			if (component2.roleData.RoleID == item.roleData.RoleID)
			{
				this.selectItemIndex = l;
				component2.transform.localScale = Vector3.one;
				break;
			}
		}
		if (this.selectItemIndex > 0)
		{
			for (int m = 0; m < this.selectItemIndex; m++)
			{
				TeamCompeteRankTeamInfo component3 = this.TeamMembersItemCollection.GetAt(m).GetComponent<TeamCompeteRankTeamInfo>();
				if (component3 != null)
				{
					this.left.Add(component3);
				}
			}
		}
		if (this.selectItemIndex + 1 < this.TeamMembersItemCollection.Count)
		{
			for (int n = this.selectItemIndex + 1; n < this.TeamMembersItemCollection.Count; n++)
			{
				TeamCompeteRankTeamInfo component4 = this.TeamMembersItemCollection.GetAt(n).GetComponent<TeamCompeteRankTeamInfo>();
				if (component4 != null)
				{
					this.right.Add(component4);
				}
			}
		}
		if (this.left != null && this.left.Count > 0)
		{
			for (int num = 0; num < this.left.Count; num++)
			{
				this.left[num].transform.localPosition = new Vector3(this.left[num].transform.localPosition.x - (float)this.offset, this.left[num].transform.localPosition.y, this.left[num].transform.localPosition.z);
				this.left[num].transform.localScale = this.localSacle;
			}
		}
		if (this.right != null && this.right.Count > 0)
		{
			for (int num2 = 0; num2 < this.right.Count; num2++)
			{
				this.right[num2].transform.localPosition = new Vector3(this.right[num2].transform.localPosition.x + (float)this.offset, this.right[num2].transform.localPosition.y, this.right[num2].transform.localPosition.z);
				this.right[num2].transform.localScale = this.localSacle;
			}
		}
	}

	private void SortByTeamleader(int teamLeaderId, List<TianTi5v5ZhanDuiRoleData> teamList)
	{
		if (teamList[0].RoleID != teamLeaderId)
		{
			int num = 0;
			for (int i = 0; i < teamList.Count; i++)
			{
				if (teamLeaderId == teamList[i].RoleID)
				{
					num = i;
					break;
				}
			}
			if (num > 0)
			{
				TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData = teamList[num];
				teamList.Remove(tianTi5v5ZhanDuiRoleData);
				teamList.Insert(0, tianTi5v5ZhanDuiRoleData);
			}
		}
	}

	private void RefreshZhanDuiInfo(TianTi5v5ZhanDuiData data, int rankId)
	{
		if (rankId <= 3)
		{
			NGUITools.SetActive(this.mSprtRanKId.gameObject, true);
			NGUITools.SetActive(this.LblZhanDuiRankId.gameObject, false);
			this.mSprtRanKId.spriteName = "rank" + rankId;
			this.mSprtRanKId.gameObject.transform.localPosition = new Vector3(-90f, this.mSprtRanKId.gameObject.transform.localPosition.y, this.mSprtRanKId.gameObject.transform.localPosition.z);
		}
		else
		{
			NGUITools.SetActive(this.mSprtRanKId.gameObject, false);
			NGUITools.SetActive(this.LblZhanDuiRankId.gameObject, true);
			this.LblZhanDuiRankId.Text = Global.GetLang("第") + rankId + Global.GetLang("名");
		}
		this.LblTName.Text = data.ZhanDuiName;
		this.LblServerName.Text = Global.GetString(new object[]
		{
			Global.GetLang("区："),
			this.GetServerName(data)
		});
		this.LblZhanLiInfo.Text = Global.GetString(new object[]
		{
			Global.GetLang("战   力："),
			data.ZhanDouLi
		});
		this.LblDuanWeiInfo.Text = Global.GetString(new object[]
		{
			Global.GetLang("段   位："),
			TeamCompeteDataManager.GetDuanWeiNameByID(data.DuanWeiId)
		});
		this.LblScoreInfo.Text = Global.GetString(new object[]
		{
			Global.GetLang("段位积分："),
			data.DuanWeiJiFen
		});
		this.mUILabelZhanDouLi.text = data.ZhanDouLi.ToString();
	}

	private string GetServerName(TianTi5v5ZhanDuiData data)
	{
		if (data.teamerList == null || data.teamerList.Count <= 0)
		{
			return string.Empty;
		}
		return Global.FormatRoleNameZoneid(data.teamerList[0].ZoneID, null, 0, 1);
	}

	private void RefreshZhanDuiPlayerRoleInfo(TianTi5v5ZhanDuiData data, TianTi5v5ZhanDuiRoleData roleData)
	{
		if (this.lastRoleInfoId == roleData.RoleID)
		{
			return;
		}
		if (roleData.ModelData == null)
		{
			return;
		}
		this.lastRoleInfoId = roleData.RoleID;
		this.LoadRoleDataInfo(TeamCompeteDataManager.GetRoleData4Selector(roleData.ModelData));
	}

	private void LoadRoleDataInfo(RoleData4Selector _RoleData4Selector)
	{
		this.ClearModal3DShow();
		Modal3DShow modal3DShow = this.mModal3DShow;
		int fashionGoodsID = Global.GetFashionGoodsID(_RoleData4Selector.FashionWingsID);
		this.roleResLoader = UIHelper.LoadRoleRes(modal3DShow, _RoleData4Selector.SettingBitFlags, _RoleData4Selector.Occupation, _RoleData4Selector.SubOccupation, _RoleData4Selector.OtherName, _RoleData4Selector.GoodsDataList, null, _RoleData4Selector.MyWingData, 1f, fashionGoodsID, null, false);
		UIHelper.SetModalPosZ(modal3DShow.transform);
	}

	private void ClearModal3DShow()
	{
		this.mModal3DShow.Clear();
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
			this.roleResLoader = null;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.ClearModal3DShow();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblRank;

	public TextBlock LblDuanWei;

	public TextBlock LblScore;

	public UISprite mSprtRanKId;

	public TextBlock LblZhanDuiRankId;

	public TextBlock LblTName;

	public TextBlock LblServerName;

	public TextBlock LblZhanLiInfo;

	public TextBlock LblDuanWeiInfo;

	public TextBlock LblScoreInfo;

	public GButton BtnClose;

	public UILabel mUILabelZhanDouLi;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	public ListBox mTeamMembersListBox;

	private ObservableCollection _TeamMembersItemCollection;

	private TeamCompeteRankItem mLastTeamCompeteRankItem;

	public Modal3DShow mModal3DShow;

	private int lastTeamId;

	private List<TeamCompeteRankTeamInfo> left = new List<TeamCompeteRankTeamInfo>();

	private List<TeamCompeteRankTeamInfo> right = new List<TeamCompeteRankTeamInfo>();

	private Vector3 localSacle = new Vector3(0.8f, 0.8f, 1f);

	private int selectItemIndex = -1;

	private int secondSelectItemIndex = -1;

	private int offset = 7;

	private int lastRoleInfoId;

	private RoleResLoader roleResLoader;
}
