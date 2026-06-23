using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class RecallGoodsEx : UserControl
{
	protected static List<string> __lwj_log_andmore_etc
	{
		get
		{
			try
			{
				if (RecallGoodsEx._lwj_log_lst_no_use_but_only_for_show_client_add_award_quene_id_state_list.Count > 50)
				{
					RecallGoodsEx._lwj_log_lst_no_use_but_only_for_show_client_add_award_quene_id_state_list.RemoveRange(0, 25);
				}
			}
			catch (Exception ex)
			{
				return RecallGoodsEx._lwj_log_lst_no_use_but_only_for_show_client_add_award_quene_id_state_list;
			}
			return RecallGoodsEx._lwj_log_lst_no_use_but_only_for_show_client_add_award_quene_id_state_list;
		}
	}

	protected ObservableCollection ItemCollection
	{
		get
		{
			return this._itemCollection;
		}
		set
		{
			this._itemCollection = value;
		}
	}

	protected string configPath
	{
		get
		{
			return this.mTab.configPath;
		}
	}

	protected XElement xml
	{
		get
		{
			return this.mTab.xml;
		}
	}

	protected List<XElement> xmlList
	{
		get
		{
			return this.mTab.xmlList;
		}
	}

	protected virtual void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		this.mTab = new RecallGoodsEx.DTab(this);
		this.InitTextInPrefabs();
		if (null != this.startTime)
		{
			this.startTime.text = this.StringActivityStartTimeYYYYMMDD_HHMMSS();
		}
		if (null != this.endTime)
		{
			this.endTime.text = this.StringActivityEndTimeYYYYMMDD_HHMMSS();
		}
		if (null != this.awardendTime)
		{
			this.awardendTime.text = this.StringActivityEndTime6PLUSYYYYMMDD_HHMMSS();
		}
		if (null != this.activityInfo)
		{
		}
		if (null != this.draggablePanel)
		{
			this.panelPosition = this.draggablePanel.gameObject.transform.localPosition;
			this.panelClipRange = this.draggablePanel.clipRange;
		}
		if (null != this.goodsList)
		{
			this._itemCollection = this.goodsList.ItemsSource;
		}
		if (null != this.dragPanel)
		{
			this.dragPanel.onDragFinished = new UIDraggablePanel.OnDragFinished(this.OnDragFinished);
		}
		if (null != this.dragPanelSpringPanel)
		{
			this.dragPanelSpringPanel.onFinished = delegate()
			{
			};
		}
		this.OnDragFinished();
		this.InitRewards();
		this.StartTimer();
	}

	protected override void OnDestroy()
	{
		this.mTab.Clear();
		this.StopTimer();
		base.OnDestroy();
	}

	public virtual void OnActive()
	{
		this.UpdateUIOnServerDataChanged();
	}

	public virtual void OnDragFinished()
	{
		if (null == this.goodsList)
		{
			return;
		}
		int num = this.goodsList.Count();
		Vector3 vector = this.dragPanel.transform.localPosition - this.panelPosition;
		Vector3 size = RecallRewardItem.GetSize(base.GetType());
		int num2 = 0;
		if (vector.y > 0f)
		{
			num2 = Mathf.RoundToInt(vector.y / size.y);
		}
		if (num2 >= num - 1)
		{
			num2 = num - 1;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (num2 == 0 || num2 == num - 1)
		{
			Vector3 vector2 = this.panelPosition + new Vector3(0f, size.y * (float)num2, 0f);
			SpringPanel.Begin(this.dragPanel.gameObject, vector2, 10f);
		}
	}

	public virtual bool InitRewards()
	{
		if (this.xml == null)
		{
			return false;
		}
		if (!ServerBufferZhaoHui.IsValid())
		{
			return false;
		}
		this._UpdateServerData();
		return true;
	}

	public void StartTimer()
	{
		this.StopTimer();
		this._Timer = new DispatcherTimer("RecallExShop_Timer");
		this._Timer.Interval = TimeSpan.FromSeconds(0.5);
		this._Timer.Tick = new DispatcherTimerEventHandler(this.Timer_Tick);
		this._Timer.Start();
	}

	public void StopTimer()
	{
		if (this._Timer == null)
		{
			return;
		}
		this._Timer.Stop();
		this._Timer.Tick = null;
		this._Timer = null;
	}

	private void Timer_Tick(object sender, object e)
	{
		if (null != this.storeLabelTime)
		{
			this.storeLabelTime.text = this.GetQiangGouLeftTimeString();
		}
		this._Timer_Tick();
	}

	protected virtual void _Timer_Tick()
	{
	}

	public virtual void SetSelectedRewardItemStatus(int signIndex)
	{
		GameObject at = this.ItemCollection.GetAt(signIndex);
		if (null != at)
		{
			RecallRewardItem component = at.GetComponent<RecallRewardItem>();
			component.PickupStatusNew = -1;
			component.RefreshUI();
		}
	}

	public string StringActivityStartTimeYYYYMMDD_HHMMSS()
	{
		DateTime dataTime = ConfigLaoWanJiaZhaoHui.PlayerRecallStartTime();
		if (ServerBufferZhaoHui.Instance.userData != null)
		{
			dataTime = ServerBufferZhaoHui.Instance.userData.TimeBegin;
		}
		return dataTime.toString("yyyy-MM-dd HH:mm:ss");
	}

	public string StringActivityEndTimeYYYYMMDD_HHMMSS()
	{
		DateTime dataTime = ConfigLaoWanJiaZhaoHui.PlayerRecallEndTime();
		if (ServerBufferZhaoHui.Instance.userData != null)
		{
			dataTime = ServerBufferZhaoHui.Instance.userData.TimeEnd;
		}
		return dataTime.toString("yyyy-MM-dd HH:mm:ss");
	}

	public string StringActivityAwardEndTimeYYYYMMDD_HHMMSS()
	{
		DateTime dataTime = ConfigLaoWanJiaZhaoHui.PlayerRecallEndTime();
		if (ServerBufferZhaoHui.Instance.userData != null)
		{
			dataTime = ServerBufferZhaoHui.Instance.userData.TimeAward;
		}
		return dataTime.toString("yyyy-MM-dd HH:mm:ss");
	}

	public string StringActivityEndTime6PLUSYYYYMMDD_HHMMSS()
	{
		DateTime dataTime = ConfigLaoWanJiaZhaoHui.PlayerRecallEndTimeSixPlus();
		return dataTime.toString("yyyy-MM-dd HH:mm:ss");
	}

	protected RecallRewardItem CreateRecallRewardItem(bool autoAddList = false)
	{
		RecallRewardItem rewardItem = U3DUtils.NEW<RecallRewardItem>();
		rewardItem.ItemCollection = rewardItem.goodsList.ItemsSource;
		rewardItem.gameObject.SetActive(true);
		rewardItem.mySun = this;
		if (autoAddList)
		{
			this._itemCollection.Add(rewardItem);
		}
		rewardItem.signBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendOpenAward(rewardItem);
		};
		return rewardItem;
	}

	protected void _UpdateServerData()
	{
		if (this.mTab.RewardIDStateDic == null)
		{
			this.mTab.RewardIDStateDic = new Dictionary<int, RecallGoodsEx.RewardIDState>();
		}
		this.mTab.Clear();
		if (!ServerBufferZhaoHui.IsValid() || ServerBufferZhaoHui.Instance.userData.AwardDic == null || !ServerBufferZhaoHui.Instance.userData.AwardDic.ContainsKey(this.mTab.ReturnAwardType))
		{
			return;
		}
		int[] array = ServerBufferZhaoHui.Instance.userData.AwardDic[this.mTab.ReturnAwardType];
		if (array == null)
		{
			return;
		}
		if (this.mTab.ReturnAwardType == 1)
		{
			for (int i = 0; i < array.Length; i += 3)
			{
				int num = array[i];
				int state = 0;
				int processnum = 0;
				if (i + 1 < array.Length)
				{
					state = array[i + 1];
				}
				if (i + 2 < array.Length)
				{
					processnum = array[i + 2];
				}
				if (!this.mTab.RewardIDStateDic.ContainsKey(num))
				{
					this.mTab.RewardIDStateDic.Add(num, new RecallGoodsEx.RewardIDState());
				}
				this.mTab.RewardIDStateDic[num].id = num;
				this.mTab.RewardIDStateDic[num].state = state;
				this.mTab.RewardIDStateDic[num].processnum = processnum;
			}
		}
		else
		{
			for (int j = 0; j < array.Length; j += 2)
			{
				int num2 = array[j];
				int state2 = 0;
				if (j + 1 < array.Length)
				{
					state2 = array[j + 1];
				}
				if (!this.mTab.RewardIDStateDic.ContainsKey(num2))
				{
					this.mTab.RewardIDStateDic.Add(num2, new RecallGoodsEx.RewardIDState());
				}
				this.mTab.RewardIDStateDic[num2].id = num2;
				this.mTab.RewardIDStateDic[num2].state = state2;
				if (this.mTab.ReturnAwardType == 2)
				{
					RecallGoodsEx.RewardIDState.lastOpenHuiGuiAward = num2;
				}
				else if (this.mTab.ReturnAwardType == 3)
				{
					RecallGoodsEx.RewardIDState.lastOpenQianDaoAward = num2;
				}
				else if (this.mTab.ReturnAwardType == 5)
				{
					RecallGoodsEx.RewardIDState.lastOpenChongZhiAward = num2;
				}
			}
		}
	}

	protected void _UpdateRewardsListStatus()
	{
		RecallGoodsEx.__lwj_log_andmore_etc.Clear();
		if (this.ItemCollection == null)
		{
			return;
		}
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			GameObject at = this.ItemCollection.GetAt(i);
			if (null != at)
			{
				RecallRewardItem component = at.GetComponent<RecallRewardItem>();
				if (component != null)
				{
					int xmlID = component.xmlID;
					component.PickupStatusNew = 0;
				}
			}
			if (this.mTab.ReturnAwardType == 3 && null != at)
			{
				RecallSignInItem component2 = at.GetComponent<RecallSignInItem>();
				if (component2 != null)
				{
					int xmlID2 = component2.xmlID;
					component2.PickupStatusNew = 0;
				}
			}
		}
		if (this.mTab.RewardIDStateDic == null)
		{
			return;
		}
		if (this.mTab.RewardIDStateDic.Count <= 0)
		{
			return;
		}
		if (this.mTab.ReturnAwardType == 1)
		{
			for (int j = 0; j < this.ItemCollection.Count; j++)
			{
				GameObject at2 = this.ItemCollection.GetAt(j);
				if (null != at2)
				{
					RecallRewardItem component3 = at2.GetComponent<RecallRewardItem>();
					int xmlID3 = component3.xmlID;
					if (this.mTab.RewardIDStateDic.ContainsKey(xmlID3))
					{
						EReturnAwardOperateState eLingquState = this.mTab.RewardIDStateDic[xmlID3].eLingquState;
						component3.PickupStatusNew = eLingquState;
						int processnum = this.mTab.RewardIDStateDic[xmlID3].processnum;
						component3.label.text = component3.Description + string.Format("({0}/{1})", processnum, component3.minRecruitNum);
						RecallGoodsEx.__lwj_log_andmore_etc.Add(string.Concat(new object[]
						{
							" [",
							this.mTab.ReturnAwardType,
							" id=",
							xmlID3,
							" state=",
							eLingquState,
							"]"
						}));
					}
				}
			}
		}
		else if (this.mTab.ReturnAwardType == 2)
		{
			if (!ServerBufferZhaoHui.IsValid() || ServerBufferZhaoHui.Instance.userData == null)
			{
				this._refreshItemListUI();
				return;
			}
			if (ServerBufferZhaoHui.Instance.userData.ReturnState >= 2 || ServerBufferZhaoHui.Instance.userData.ReturnState == -7)
			{
				int lastOpenHuiGuiAward = RecallGoodsEx.RewardIDState.lastOpenHuiGuiAward;
				for (int k = 0; k < this.ItemCollection.Count; k++)
				{
					GameObject at3 = this.ItemCollection.GetAt(k);
					if (null != at3)
					{
						RecallRewardItem component4 = at3.GetComponent<RecallRewardItem>();
						int xmlID4 = component4.xmlID;
						EReturnAwardOperateState ereturnAwardOperateState;
						if (ServerBufferZhaoHui.Instance.userData.ReturnState == -7)
						{
							ereturnAwardOperateState = 2;
							component4.PickupStatusNew = ereturnAwardOperateState;
						}
						else if (xmlID4 <= lastOpenHuiGuiAward)
						{
							ereturnAwardOperateState = -1;
							component4.PickupStatusNew = ereturnAwardOperateState;
						}
						else if (Global.Data.roleData.VIPLevel >= component4.minVip)
						{
							ereturnAwardOperateState = 1;
							component4.PickupStatusNew = ereturnAwardOperateState;
						}
						else
						{
							ereturnAwardOperateState = 0;
							component4.PickupStatusNew = ereturnAwardOperateState;
						}
						RecallGoodsEx.__lwj_log_andmore_etc.Add(string.Concat(new object[]
						{
							" [",
							this.mTab.ReturnAwardType,
							" id=",
							xmlID4,
							" state=",
							ereturnAwardOperateState,
							"]"
						}));
					}
				}
			}
		}
		else if (this.mTab.ReturnAwardType == 3)
		{
			if (!ServerBufferZhaoHui.IsValid() || ServerBufferZhaoHui.Instance.userData == null)
			{
				this._refreshItemListUI();
				return;
			}
			if (ServerBufferZhaoHui.Instance.userData.ReturnState >= 2 || ServerBufferZhaoHui.Instance.userData.ReturnState == -7)
			{
				int lastOpenQianDaoAward = RecallGoodsEx.RewardIDState.lastOpenQianDaoAward;
				for (int l = 0; l < this.ItemCollection.Count; l++)
				{
					GameObject at4 = this.ItemCollection.GetAt(l);
					if (null != at4)
					{
						RecallSignInItem component5 = at4.GetComponent<RecallSignInItem>();
						int xmlID5 = component5.xmlID;
						EReturnAwardOperateState ereturnAwardOperateState2;
						if (ServerBufferZhaoHui.Instance.userData.ReturnState == -7)
						{
							ereturnAwardOperateState2 = 2;
							component5.PickupStatusNew = ereturnAwardOperateState2;
						}
						else if (xmlID5 <= lastOpenQianDaoAward)
						{
							ereturnAwardOperateState2 = -1;
							component5.PickupStatusNew = ereturnAwardOperateState2;
						}
						else
						{
							int unionLevel = ServerBufferZhaoHui.Instance.UnionLevel;
							int todayReturnDayCount = ServerBufferZhaoHui.Instance.TodayReturnDayCount;
							if (unionLevel < component5.MinUnionLevel || unionLevel > component5.MaxUnionLevel || todayReturnDayCount < component5.day)
							{
								ereturnAwardOperateState2 = 0;
								component5.PickupStatusNew = ereturnAwardOperateState2;
							}
							else
							{
								ereturnAwardOperateState2 = 1;
								component5.PickupStatusNew = ereturnAwardOperateState2;
							}
						}
						RecallGoodsEx.__lwj_log_andmore_etc.Add(string.Concat(new object[]
						{
							" [",
							this.mTab.ReturnAwardType,
							" id=",
							xmlID5,
							" state=",
							ereturnAwardOperateState2,
							"]"
						}));
					}
				}
			}
		}
		else if (this.mTab.ReturnAwardType == 5)
		{
			if (!ServerBufferZhaoHui.IsValid() || ServerBufferZhaoHui.Instance.userData == null)
			{
				this._refreshItemListUI();
				return;
			}
			if (ServerBufferZhaoHui.Instance.userData.ReturnState >= 2 || ServerBufferZhaoHui.Instance.userData.ReturnState == -7)
			{
				int lastOpenChongZhiAward = RecallGoodsEx.RewardIDState.lastOpenChongZhiAward;
				for (int m = 0; m < this.ItemCollection.Count; m++)
				{
					GameObject at5 = this.ItemCollection.GetAt(m);
					if (null != at5)
					{
						RecallLeiChongItem component6 = at5.GetComponent<RecallLeiChongItem>();
						int xmlID6 = component6.xmlID;
						EReturnAwardOperateState ereturnAwardOperateState3;
						if (ServerBufferZhaoHui.Instance.userData.ReturnState == -7)
						{
							ereturnAwardOperateState3 = 2;
							component6.PickupStatusNew = ereturnAwardOperateState3;
						}
						else if (xmlID6 <= lastOpenChongZhiAward)
						{
							ereturnAwardOperateState3 = -1;
							component6.PickupStatusNew = ereturnAwardOperateState3;
						}
						else if (ServerBufferZhaoHui.Instance.userData.LeiJiChongZhi >= component6.zuanshiNumber)
						{
							ereturnAwardOperateState3 = 1;
							component6.PickupStatusNew = ereturnAwardOperateState3;
						}
						else
						{
							ereturnAwardOperateState3 = 0;
							component6.PickupStatusNew = ereturnAwardOperateState3;
						}
						RecallGoodsEx.__lwj_log_andmore_etc.Add(string.Concat(new object[]
						{
							" [",
							this.mTab.ReturnAwardType,
							" id=",
							xmlID6,
							" state=",
							ereturnAwardOperateState3,
							"]"
						}));
					}
				}
			}
		}
		else if (this.mTab.ReturnAwardType == 4)
		{
			this.storeLabelMoney.text = Global.Data.roleData.UserMoney.ToString();
			MallItem mallItem = null;
			for (int n = 0; n < this.ItemCollection.Count; n++)
			{
				mallItem = this.ItemCollection.GetAt(n).GetComponent<MallItem>();
				if (null != mallItem)
				{
					int num = 0;
					foreach (int num2 in this.mTab.RewardIDStateDic.Keys)
					{
						if (mallItem.ItemID == num2)
						{
							num = this.mTab.RewardIDStateDic[num2].state;
							break;
						}
					}
					mallItem.ShengYuShu = (mallItem.XianGouShu.SafeToInt32(0) - num).ToString();
				}
			}
		}
		this._refreshItemListUI();
	}

	private void _refreshItemListUI()
	{
		if (this.ItemCollection == null)
		{
			return;
		}
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			GameObject at = this.ItemCollection.GetAt(i);
			if (null != at)
			{
				RecallRewardItem component = at.GetComponent<RecallRewardItem>();
				if (component != null)
				{
					component.RefreshUI();
				}
			}
			if (this.mTab.ReturnAwardType == 3 && null != at)
			{
				RecallSignInItem component2 = at.GetComponent<RecallSignInItem>();
				if (component2 != null)
				{
					component2.RefreshUI();
				}
			}
			if (this.mTab.ReturnAwardType == 5 && null != at)
			{
				RecallLeiChongItem component3 = at.GetComponent<RecallLeiChongItem>();
				if (component3 != null)
				{
					component3.RefreshUI();
				}
			}
		}
	}

	public void UpdateUIOnServerDataChanged()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		this._UpdateServerData();
		this._UpdateRewardsListStatus();
		if (this.mTab.ReturnAwardType == 4)
		{
		}
	}

	public string GetQiangGouLeftTimeString()
	{
		string result = Global.GetLang("已结束");
		if (ServerBufferZhaoHui.Instance.userData == null || false)
		{
			return result;
		}
		TimeSpan timeSpan = ServerBufferZhaoHui.Instance.userData.TimeAward - Global.GetCorrectDateTime();
		if (timeSpan.Days > 0)
		{
			result = StringUtil.substitute(Global.GetLang("{0}天{1}小时{2}分钟{3}秒"), new object[]
			{
				timeSpan.Days,
				timeSpan.Hours,
				timeSpan.Minutes,
				timeSpan.Seconds
			});
		}
		else if (timeSpan.Hours > 0)
		{
			result = StringUtil.substitute(Global.GetLang("{0}小时{1}分钟{2}秒"), new object[]
			{
				timeSpan.Hours,
				timeSpan.Minutes,
				timeSpan.Seconds
			});
		}
		else if (timeSpan.Minutes > 0 || timeSpan.Seconds > 0)
		{
			result = StringUtil.substitute(Global.GetLang("{0}分钟{1}秒"), new object[]
			{
				timeSpan.Minutes,
				timeSpan.Seconds
			});
		}
		return result;
	}

	public static string GetRecallInfo()
	{
		string text = string.Empty;
		if (ServerBufferZhaoHui.Instance.userData == null)
		{
			text += string.Format("{0}", "userData = null ");
			return text;
		}
		UserReturnData userData = ServerBufferZhaoHui.Instance.userData;
		text += string.Format("ActivityIsOpen = {0} ", userData.ActivityIsOpen);
		text += string.Format("TimeBegin = {0} ", userData.TimeBegin);
		text += string.Format("TimeEnd = {0} ", userData.TimeEnd);
		text += string.Format("TimeAward = {0} ", userData.TimeAward);
		text += string.Format("RecallCode = {0} ", userData.RecallCode);
		text += string.Format("RecallZoneID = {0} ", userData.RecallZoneID);
		text += string.Format("RecallRoleID = {0} ", userData.RecallRoleID);
		text += string.Format("Level = {0} ", userData.Level);
		text += string.Format("Vip = {0} ", userData.Vip);
		text += string.Format("TimeReturn = {0} ", userData.TimeReturn);
		text += string.Format("ZhuanSheng = {0} ", userData.ZhuanSheng);
		text += string.Format("DengJi = {0} ", userData.DengJi);
		text += string.Format("ServerReturnState = {0} ", userData.ReturnState.ToString());
		text += string.Format("ClientReturnState = {0} ", ServerBufferZhaoHui.Instance.ClientReturnState.ToString());
		text += string.Format(Global.GetLang("客户端计算的回归天数 = {0} "), ServerBufferZhaoHui.Instance.TodayReturnDayCount.ToString());
		text += string.Format(Global.GetLang("服务器提供的奖励信息 {0} "), Global.GetLang("如下"));
		foreach (int num in ServerBufferZhaoHui.Instance.userData.AwardDic.Keys)
		{
			text += string.Format(Global.GetLang("奖励{0}:"), num.ToString());
			if (ServerBufferZhaoHui.Instance.userData.AwardDic[num] != null)
			{
				foreach (int num2 in ServerBufferZhaoHui.Instance.userData.AwardDic[num])
				{
					text = text + num2 + ":";
				}
			}
			else
			{
				text += "none";
			}
		}
		text += string.Format(Global.GetLang(" 客户端显示统计信息 {0}:"), string.Empty);
		foreach (string text2 in RecallGoodsEx.__lwj_log_andmore_etc)
		{
			text += text2;
		}
		text += string.Format(" {0} ", string.Empty);
		return text;
	}

	protected void SendOpenAward(RecallRewardItem rewardItem)
	{
		ServerBufferZhaoHui.Instance.SendOpenAward(this.mTab.ReturnAwardType, rewardItem.xmlID, 1);
	}

	protected void SendOpenAward(int xmlID)
	{
		ServerBufferZhaoHui.Instance.SendOpenAward(this.mTab.ReturnAwardType, xmlID, 1);
	}

	protected static List<string> _lwj_log_lst_no_use_but_only_for_show_client_add_award_quene_id_state_list = new List<string>();

	protected RecallGoodsEx.DTab mTab;

	public DPSelectedItemEventHandler DPSelectedItem;

	public UILabel storeLabel1;

	public UILabel storeLabel2;

	public UILabel storeLabelTime;

	public UILabel storeLabelMoney;

	public UILabel startTime;

	public UILabel endTime;

	public UILabel awardendTime;

	public UILabel activityInfo;

	public UIPanel draggablePanel;

	public UIDraggablePanel dragPanel;

	public SpringPanel dragPanelSpringPanel;

	public ListBox goodsList;

	public Vector3 panelPosition = default(Vector3);

	public Vector4 panelClipRange = default(Vector4);

	public Vector2 goodListItemSize = default(Vector2);

	protected ObservableCollection _itemCollection;

	private DispatcherTimer _Timer;

	public static UserReturnXmlData m_UserReturnXmlData;

	public class RewardIDState
	{
		public EReturnAwardOperateState eLingquState
		{
			get
			{
				return this.state;
			}
		}

		public int id;

		public int state;

		public int processnum;

		public static int lastOpenHuiGuiAward = -1;

		public static int lastOpenQianDaoAward = -1;

		public static int lastOpenChongZhiAward = -1;
	}

	protected class DTab
	{
		public DTab(RecallGoodsEx go)
		{
			this._go = go;
			this.init();
		}

		public void Clear()
		{
			foreach (int num in this.RewardIDStateDic.Keys)
			{
				this.RewardIDStateDic[num].state = 0;
				this.RewardIDStateDic[num].processnum = 0;
			}
			if (this.ReturnAwardType != 1)
			{
				if (this.ReturnAwardType == 2)
				{
					RecallGoodsEx.RewardIDState.lastOpenHuiGuiAward = -1;
				}
				else if (this.ReturnAwardType == 3)
				{
					RecallGoodsEx.RewardIDState.lastOpenQianDaoAward = -1;
				}
				else if (this.ReturnAwardType == 5)
				{
					RecallGoodsEx.RewardIDState.lastOpenChongZhiAward = -1;
				}
			}
		}

		public string IDKeyString
		{
			get
			{
				return this._IDKeyString;
			}
		}

		public EReturnAwardType ReturnAwardType
		{
			get
			{
				return this._ReturnAwardType;
			}
		}

		public string configPath
		{
			get
			{
				if (this._go.GetType() == typeof(RecallRewards))
				{
					this._IDKeyString = "RecruitOld";
					this._ConfigPath = "RecruitOld.xml";
					this._ReturnAwardType = 1;
				}
				if (this._go.GetType() == typeof(MyReferee))
				{
					this._IDKeyString = string.Empty;
					this._ConfigPath = string.Empty;
					this._ReturnAwardType = 1;
				}
				if (this._go.GetType() == typeof(RecallLeiChong))
				{
					this._IDKeyString = "OldHuoDongChongZhiGift";
					this._ConfigPath = "OldHuoDongchongzhiGift.xml";
					this._ReturnAwardType = 5;
				}
				if (this._go.GetType() == typeof(RecallGiftSet))
				{
					this._IDKeyString = "OldLogin";
					this._ConfigPath = "OldLogin.xml";
					this._ReturnAwardType = 2;
				}
				if (this._go.GetType() == typeof(RecallSignIn))
				{
					this._IDKeyString = "OldHuoDongLoginNumGift";
					this._ConfigPath = "OldHuoDongLoginNumGift.xml";
					this._ReturnAwardType = 3;
				}
				if (this._go.GetType() == typeof(RecallShop))
				{
					this._IDKeyString = "OldStore";
					this._ConfigPath = "OldStore.xml";
					this._ReturnAwardType = 4;
				}
				return this._ConfigPath;
			}
		}

		private void init()
		{
			if (this.xml != null)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.configPath))
			{
				return;
			}
			if (RecallGoodsEx.m_UserReturnXmlData != null)
			{
				for (int i = 0; i < RecallGoodsEx.m_UserReturnXmlData.XmlNameList.Count; i++)
				{
					if (RecallGoodsEx.m_UserReturnXmlData.XmlNameList[i].Equals(this.configPath))
					{
						this.xml = XElement.Parse(RecallGoodsEx.m_UserReturnXmlData.XmlList[i]);
					}
				}
			}
			else
			{
				this.xml = Global.GetIsolateResXml(this.configPath);
			}
			if (this.RewardIDStateDic == null)
			{
				this.RewardIDStateDic = new Dictionary<int, RecallGoodsEx.RewardIDState>();
				this.xmlList = Global.GetXElementList(this.xml, this._IDKeyString);
				for (int j = 0; j < this.xmlList.Count; j++)
				{
					XElement xelement = this.xmlList[j];
					int num = Global.SafeConvertToInt32(Global.GetXElementAttributeStr(xelement, "ID"));
					if (!this.RewardIDStateDic.ContainsKey(num))
					{
						this.RewardIDStateDic.Add(num, new RecallGoodsEx.RewardIDState());
					}
				}
				return;
			}
		}

		private RecallGoodsEx _go;

		public Dictionary<int, RecallGoodsEx.RewardIDState> RewardIDStateDic;

		private string _ConfigPath = string.Empty;

		private string _IDKeyString = string.Empty;

		private EReturnAwardType _ReturnAwardType = 1;

		public XElement xml;

		public List<XElement> xmlList;
	}
}
