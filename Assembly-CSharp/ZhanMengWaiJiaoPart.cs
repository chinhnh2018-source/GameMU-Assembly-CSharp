using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhanMengWaiJiaoPart : UserControl
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

	private void InitTextInPrefabs()
	{
		this.btnFriend.Text = Global.GetLang("我的盟友");
		this.btnRequest.Text = Global.GetLang("结盟请求");
		this.btnInfo.Text = Global.GetLang("结盟信息");
		this.title.text = Global.GetLang("战 盟");
		this.title.transform.localPosition = new Vector3(this.title.transform.localPosition.x, 235f, this.title.transform.localPosition.z);
		this.title.transform.localScale = new Vector3(30f, 30f, 1f);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.isFirstLogin = true;
		this.sumMengYouCount = ConvertExt.SafeConvertToInt32(ConfigSystemParam.GetSystemParamByName("AlignNum", true));
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			base.transform.parent = null;
			Object.Destroy(base.transform.gameObject);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, null);
			}
		};
		this.btnFriend.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetTabPanel(1);
		};
		this.btnRequest.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetTabPanel(2);
		};
		this.btnInfo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetTabPanel(3);
		};
		if (Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
		{
			ActivityTipManager.RegActivityTipItem(14112, delegate(int s, ActivityTipItem e)
			{
				this.isShowRequestTip = e.IsActive;
				if (this.isShowRequestTip)
				{
					if (!this.isFirstLogin)
					{
						this.RefreshAllyItemsData();
					}
					this.requestTipObj.SetActive(true);
				}
				else
				{
					this.requestTipObj.SetActive(this.isShowRequestTip);
				}
			});
			ActivityTipManager.RegActivityTipItem(14113, delegate(int s, ActivityTipItem e)
			{
				this.isShowInfoTip = e.IsActive;
				if (this.isShowInfoTip)
				{
					if (!this.isFirstLogin)
					{
						this.RefreshAllyItemsData();
					}
					this.infoTipObj.SetActive(true);
				}
				else
				{
					this.infoTipObj.SetActive(this.isShowInfoTip);
				}
			});
		}
	}

	public void InitTabPanel()
	{
		this.InitTextureBg();
		this.SetTabPanel(1);
	}

	private void InitTextureBg()
	{
		this.bg.URL = "NetImages/GameRes/Images/ZhanMengWaiJiao/CommomBg.png";
	}

	private void SetTabPanel(int type)
	{
		if (this.UnebaleContinueClick(type))
		{
			return;
		}
		switch (type)
		{
		case 1:
			this.requestMengYouType = 1;
			this.preTabId = 1;
			this.ShowJieMengTip(1);
			this.SetBtnStat(this.btnFriend);
			this.HidePanel(type);
			if (!Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
			{
				this.SetButtonUnenable(this.faQiJieMengTip.btnFaQiJieMeng);
			}
			this.ShowTypeList(1);
			this.faQiJieMengTip.SetMengYouCount(this.mengyouCount, this.sumMengYouCount);
			break;
		case 2:
			if (!Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
			{
				Super.HintMainText(Global.GetLang("只有首领才能处理结盟请求"), 10, 3);
				return;
			}
			this.requestMengYouType = 2;
			this.preTabId = 2;
			this.ShowJieMengTip(2);
			this.SetBtnStat(this.btnRequest);
			this.HidePanel(type);
			this.ShowTypeList(2);
			this.jieMengQingQiu.SetMengYouCount(this.mengyouCount, this.sumMengYouCount);
			break;
		case 3:
			this.requestMengYouType = 3;
			this.preTabId = 3;
			this.ShowJieMengTip(3);
			this.SetBtnStat(this.btnInfo);
			this.HidePanel(type);
			this.ShowTypeList(3);
			break;
		}
	}

	private void ShowTypeList(int tabID)
	{
		Super.ShowNetWaiting(null);
		switch (tabID)
		{
		case 1:
			GameInstance.Game.SendZhanMengWaiJiaoMengYouRequest(tabID);
			break;
		case 2:
			GameInstance.Game.SendZhanMengWaiJiaoMengYouRequest(tabID);
			break;
		case 3:
			GameInstance.Game.SendZhanMengWaiJiaoLogRequest();
			break;
		}
	}

	public void NotifyRefreshAllyData(List<AllyData> dataList)
	{
		Super.HideNetWaiting();
		if (this.isFirstLogin)
		{
			this.isFirstLogin = false;
		}
		if (dataList == null || dataList.Count == 0)
		{
			this.ClearZhanMengListData();
			this.ShowFriendOrRequestPanel();
			Super.HintMainText(Global.GetLang("暂时没有数据"), 10, 3);
			return;
		}
		this.ClearZhanMengListData();
		this.zhanMengWaiJiaoDataList = dataList;
		this.RefreshTabPanel();
	}

	private void ShowFriendOrRequestPanel()
	{
		if (this.requestMengYouType == 1)
		{
			this.mengyouCount = 0;
			this.preMengYouCount = 0;
			this.faQiJieMengTip.SetMengYouCount(this.mengyouCount, this.sumMengYouCount);
			this.ShowFriendPanel();
		}
		if (this.requestMengYouType == 2)
		{
			this.ShowRequestPanel();
		}
	}

	private void RefreshTabPanel()
	{
		this.mengyouCount = this.GetCurrentMengYouCount();
		if (this.requestMengYouType == 1)
		{
			this.preMengYouCount = this.mengyouCount;
			this.faQiJieMengTip.SetMengYouCount(this.mengyouCount, this.sumMengYouCount);
			if (this.mengyouCount >= this.sumMengYouCount)
			{
				this.faQiJieMengTip.transform.gameObject.SetActive(false);
				this.jieMengQingQiu.transform.gameObject.SetActive(true);
				this.jieMengQingQiu.SetMengYouCount(this.mengyouCount, this.sumMengYouCount);
			}
			else
			{
				this.faQiJieMengTip.transform.gameObject.SetActive(true);
				this.jieMengQingQiu.transform.gameObject.SetActive(false);
			}
			this.ShowFriendPanel();
		}
		else if (this.requestMengYouType == 2)
		{
			this.jieMengQingQiu.SetMengYouCount(this.preMengYouCount, this.sumMengYouCount);
			this.ShowRequestPanel();
		}
	}

	public void ShowFriendPanel()
	{
		if (this.m_friendPanel == null)
		{
			this.m_friendPanel = U3DUtils.NEW<ZhanMengWaiJiaoFriendPanel>();
			U3DUtils.AddChild(this.pnl, this.m_friendPanel.gameObject, true);
			this.m_friendPanel.gameObject.transform.localPosition = new Vector3(0f, 0f, -1f);
			this.m_friendPanel.ClearData();
			this.m_friendPanel.InitData(this.zhanMengWaiJiaoDataList);
		}
		else
		{
			this.m_friendPanel.IsShow(true);
			this.m_friendPanel.InitData(this.zhanMengWaiJiaoDataList);
		}
	}

	public void ShowRequestPanel()
	{
		if (null == this.m_requestPanel)
		{
			this.m_requestPanel = U3DUtils.NEW<ZhanMengWaiJiaoRequestPanel>();
			U3DUtils.AddChild(this.pnl, this.m_requestPanel.gameObject, true);
			this.m_requestPanel.gameObject.transform.localPosition = new Vector3(0f, 0f, -1f);
			this.m_requestPanel.ClearData();
			this.m_requestPanel.InitData(this.zhanMengWaiJiaoDataList);
		}
		else
		{
			this.m_requestPanel.IsShow(true);
			this.m_requestPanel.InitData(this.zhanMengWaiJiaoDataList);
		}
	}

	public void HidePanel(int tabId)
	{
		switch (tabId)
		{
		case 1:
			if (this.m_requestPanel != null)
			{
				this.m_requestPanel.ClearData();
				this.m_requestPanel.IsShow(false);
			}
			if (this.m_logPanel != null)
			{
				this.m_logPanel.ClearData();
				this.m_logPanel.IsShow(false);
			}
			break;
		case 2:
			if (this.m_friendPanel != null)
			{
				this.m_friendPanel.ClearData();
				this.m_friendPanel.IsShow(false);
			}
			if (this.m_logPanel != null)
			{
				this.m_logPanel.ClearData();
				this.m_logPanel.IsShow(false);
			}
			break;
		case 3:
			if (this.m_requestPanel != null)
			{
				this.m_requestPanel.ClearData();
				this.m_requestPanel.IsShow(false);
			}
			if (this.m_friendPanel != null)
			{
				this.m_friendPanel.ClearData();
				this.m_friendPanel.IsShow(false);
			}
			break;
		}
	}

	public void RefreshAllyItemsData()
	{
		this.ClearZhanMengListData();
		if (this.m_friendPanel != null)
		{
			this.m_friendPanel.ClearData();
		}
		if (this.m_logPanel != null)
		{
			this.m_logPanel.ClearData();
		}
		if (this.m_requestPanel != null)
		{
			this.m_requestPanel.ClearData();
		}
		this.ShowTypeList(this.requestMengYouType);
	}

	public void NotifyRefreshAllyLogData(List<AllyLogData> dataList)
	{
		Super.HideNetWaiting();
		this.zhanMengWaiJiaoLogList.Clear();
		if (dataList == null || dataList.Count == 0)
		{
			Super.HintMainText(Global.GetLang("暂时没有数据"), 10, 3);
			return;
		}
		this.zhanMengWaiJiaoLogList = dataList;
		if (this.m_logPanel == null)
		{
			this.m_logPanel = U3DUtils.NEW<ZhanMengWaiJiaoLogPanel>();
			U3DUtils.AddChild(this.pnl, this.m_logPanel.gameObject, true);
			this.m_logPanel.gameObject.transform.localPosition = new Vector3(0f, 0f, -1f);
			this.m_logPanel.ClearData();
			this.m_logPanel.InitData(this.zhanMengWaiJiaoLogList);
		}
		else
		{
			this.m_logPanel.IsShow(true);
			this.m_logPanel.InitData(this.zhanMengWaiJiaoLogList);
		}
	}

	public void NotifyFaQiJieMengResult(int result)
	{
		Super.HideNetWaiting();
		switch (result + 13)
		{
		case 0:
			Super.HintMainText(Global.GetLang("战盟id错误"), 10, 3);
			break;
		case 1:
			Super.HintMainText(Global.GetLang("发起请求超出上限"), 10, 3);
			break;
		case 2:
			Super.HintMainText(Global.GetLang("结盟数量超出上限"), 10, 3);
			break;
		case 3:
			Super.HintMainText(Global.GetLang("已经向对方发起了结盟"), 10, 3);
			break;
		case 4:
			Super.HintMainText(Global.GetLang("已经结盟"), 10, 3);
			break;
		case 5:
			Super.HintMainText(Global.GetLang("战盟资金不足"), 10, 3);
			break;
		case 7:
			Super.HintMainText(Global.GetLang("不能同自己战盟结盟"), 10, 3);
			break;
		case 10:
			Super.HintMainText(Global.GetLang("战盟名字不存在或等级未达到5级"), 10, 3);
			break;
		case 11:
			Super.HintMainText(Global.GetLang("输入服务器有误，请重试！"), 10, 3);
			break;
		case 12:
			Super.HintMainText(Global.GetLang("战盟名字不存在或等级未达到5级"), 10, 3);
			break;
		case 14:
			Super.HintMainText(Global.GetLang("结盟请求已发出"), 10, 3);
			this.faQiJieMengTip.faQiJieMeng.NotifyResult();
			this.RefreshAllyItemsData();
			break;
		}
	}

	public void NotifyCancelJieMengRequestResult(int result)
	{
		Super.HideNetWaiting();
		if (result == 30)
		{
			Super.HintMainText(Global.GetLang("取消结盟请求失败"), 10, 3);
		}
		else if (result == 31)
		{
			Super.HintMainText(Global.GetLang("取消结盟请求成功"), 10, 3);
		}
		this.RefreshAllyItemsData();
	}

	public void NotifyAgreeOrRefuseResult(int result)
	{
		Super.HideNetWaiting();
		if (result == -11)
		{
			if (this.sumMengYouCount == 0)
			{
				this.sumMengYouCount = ConvertExt.SafeConvertToInt32(ConfigSystemParam.GetSystemParamByName("AlignNum", true));
			}
			Super.HintMainText(string.Format(Global.GetLang("最多只能跟{0}个战盟结盟"), this.sumMengYouCount), 10, 3);
			return;
		}
		if (result == 12)
		{
			Super.HintMainText(Global.GetLang("结盟成功"), 10, 3);
		}
		this.RefreshAllyItemsData();
	}

	public void NotifyCancelRelationshipResult(int result)
	{
		Super.HideNetWaiting();
		if (null != this.m_friendPanel)
		{
			this.m_friendPanel.RefreshUI(result);
		}
		this.RefreshAllyItemsData();
	}

	public void NotifyMengYouCountInRequest(int result)
	{
		if (result >= this.sumMengYouCount)
		{
			result = this.sumMengYouCount;
		}
		this.preMengYouCount = result;
		this.mengyouCount = result;
		this.jieMengQingQiu.SetMengYouCount(result, this.sumMengYouCount);
	}

	private void PopCancelJieMengWindow(int id, string zhanMengName)
	{
		this.jieChuMengYueWindow.SetActive(true);
		this.jieChuMengYue.SetContent(zhanMengName);
		this.jieChuMengYue.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			GameInstance.Game.SendJieChuJieMengRequest(id);
		};
	}

	private void SetBtnStat(GButton btn)
	{
		if (this.tempBtn != null)
		{
			if (this.tempBtn == btn)
			{
				btn.Label.color = NGUIMath.HexToColorEx(16766048U);
				return;
			}
			btn.Pressed = true;
			btn.Label.color = NGUIMath.HexToColorEx(16766048U);
			this.tempBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
			this.tempBtn.Pressed = false;
			this.tempBtn = btn;
		}
		else
		{
			btn.Label.color = NGUIMath.HexToColorEx(16766048U);
			btn.Pressed = true;
			this.tempBtn = btn;
		}
	}

	private void SetButtonUnenable(GButton btn)
	{
		btn.isEnabled = false;
	}

	private void ClearZhanMengListData()
	{
		this.zhanMengWaiJiaoDataList.Clear();
		if (null != this.m_friendPanel)
		{
			this.m_friendPanel.ClearData();
		}
		if (this.m_requestPanel != null)
		{
			this.m_requestPanel.ClearData();
		}
	}

	private new void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(14113, null);
		ActivityTipManager.RegActivityTipItem(14112, null);
	}

	private int GetCurrentMengYouCount()
	{
		this.curMengYouCount = 0;
		int count = this.zhanMengWaiJiaoDataList.Count;
		if (count <= 0)
		{
			return 0;
		}
		for (int i = 0; i < count; i++)
		{
			AllyData allyData = this.zhanMengWaiJiaoDataList[i];
			if (allyData.LogState == 12)
			{
				this.curMengYouCount++;
			}
		}
		return this.curMengYouCount;
	}

	private bool UnebaleContinueClick(int tabId)
	{
		return this.preTabId == tabId;
	}

	public void ShowJieMengTip(int tabId)
	{
		this.jieMengBtnObj.SetActive(tabId == 1);
		this.JieMengQingQiuTip.SetActive(tabId == 2);
	}

	public static string GetFontColorContentForChinese(string content, string tmpColor = "ffffff")
	{
		return Global.GetColorStringForNGUIText(new object[]
		{
			tmpColor,
			Global.GetLang(content)
		});
	}

	public GButton btnClose;

	public GButton btnFriend;

	public GButton btnRequest;

	public GButton btnInfo;

	private GButton tempBtn;

	public UILabel title;

	public GameObject jieChuMengYueWindow;

	private ZhanMengJieChuMengYueWindow jieChuMengYue;

	public GameObject jieMengBtnObj;

	public FaQiJieMengTip faQiJieMengTip;

	public GameObject JieMengQingQiuTip;

	public JieMengQingQiuTip jieMengQingQiu;

	public GameObject requestTipObj;

	public GameObject infoTipObj;

	private List<AllyData> zhanMengWaiJiaoDataList = new List<AllyData>();

	private List<AllyLogData> zhanMengWaiJiaoLogList = new List<AllyLogData>();

	private bool isShowRequestTip;

	private bool isShowInfoTip;

	private int sumMengYouCount = 5;

	private int requestMengYouType;

	private int curMengYouCount;

	private int mengyouCount;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ShowNetImage bg;

	private int preTabId;

	private bool isFirstLogin;

	private ObservableCollection _ItemCollection;

	public GameObject pnl;

	public ZhanMengWaiJiaoLogPanel m_logPanel;

	public ZhanMengWaiJiaoFriendPanel m_friendPanel;

	public ZhanMengWaiJiaoRequestPanel m_requestPanel;

	private int preMengYouCount;

	private enum TabType
	{
		FriendTab = 1,
		RequestTab,
		LogTab
	}
}
