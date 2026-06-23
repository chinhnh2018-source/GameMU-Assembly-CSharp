using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class XuanFuPartFirst : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnAllServerLabel.text = Global.GetLang("所有服务器");
		this.m_BtnRefreshRecordLabel.text = Global.GetLang("刷新");
		this.m_LabZhuTiFuTiShi.text = Global.GetLang("不同主题服间跨服功能不连通");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (XuanFuPartFirst.RemainRefreshTime > 0)
		{
			this.m_BtnRefreshRecord.isEnabled = false;
			this.StartUITimer();
		}
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			};
		}
		if (this.m_BtnAllServer != null)
		{
			this.m_BtnAllServer.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 3
				});
			};
		}
		if (this.m_BtnRefreshRecord != null)
		{
			this.m_BtnRefreshRecord.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.m_BtnRefreshRecord.isEnabled = false;
				XuanFuPartFirst.RemainRefreshTime = 30;
				this.StartUITimer();
				base.StartCoroutine<bool>(this.GetRecordData());
			};
		}
	}

	public new void OnDestroy()
	{
		this.StopTimer();
	}

	protected void StartUITimer()
	{
		if (this.UITimer == null)
		{
			this.UITimer = new DispatcherTimer("XuanFuRefresh_Timer");
			this.UITimer.Interval = TimeSpan.FromSeconds(1.0);
			this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		}
		this.UITimer.Start();
	}

	private void StopTimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Tick = null;
			this.UITimer.Stop();
			this.UITimer = null;
		}
	}

	protected void UITimer_Tick(object sender, object e)
	{
		if (XuanFuPartFirst.RemainRefreshTime > 0)
		{
			this.m_BtnRefreshRecordLabel.text = string.Format(Global.GetLang("{0}秒"), XuanFuPartFirst.RemainRefreshTime);
			XuanFuPartFirst.RemainRefreshTime--;
		}
		else
		{
			this.m_BtnRefreshRecord.isEnabled = true;
			this.m_BtnRefreshRecordLabel.text = Global.GetLang("刷新");
			this.StopTimer();
		}
	}

	public IEnumerator GetRecordData()
	{
		string strUID = Global.StringReplaceAll(Super.GetXapParamByName("uid", string.Empty), ":", string.Empty);
		if (string.IsNullOrEmpty(strUID) || strUID == "-1")
		{
			Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("暂未登录平台，请登录后再试..."), -1, -1, -1, -1, false);
			MUDebug.LogError<string>(new string[]
			{
				"暂无玩家uid值"
			});
			yield break;
		}
		Super.ShowNetWaiting(null);
		string url = Global.ServerListURLSecond + "GetUserServerId.aspx";
		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField("userId", strUID);
		WWW www = new WWW(url, wwwForm);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Super.HideNetWaiting();
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接(GetUserServerId),请检查您的网络是否正常！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				Application.Quit();
				return true;
			};
			yield break;
		}
		string serverIDs = www.text;
		if (string.IsNullOrEmpty(serverIDs))
		{
			Super.HideNetWaiting();
			yield break;
		}
		PlayerPrefs.SetString(strUID, serverIDs);
		www.Dispose();
		www = null;
		url = Global.ServerListURL + "GetUserServerListZt.aspx";
		wwwForm = new WWWForm();
		wwwForm.AddField("serverId", serverIDs);
		www = new WWW(url, wwwForm);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Super.HideNetWaiting();
			GChildWindow messageBoxWindow2 = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接(GetUserServerList),请检查您的网络是否正常！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow2.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow2.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow2);
				Application.Quit();
				return true;
			};
			yield break;
		}
		ZtBuffServerListDataEx listDataEx = DataHelper.BytesToObject<ZtBuffServerListDataEx>(www.bytes, 0, www.bytes.Length);
		if (listDataEx == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"没有找到远程的ServerList.xml"
			});
			Super.HideNetWaiting();
			yield break;
		}
		url = Global.ServerListCrossPlatfomURL + "GetUserServerListZt.aspx";
		WWW wwwCrossPlatform = new WWW(url, wwwForm);
		yield return wwwCrossPlatform;
		ZtBuffServerListDataEx listDataExCrossPlatform = null;
		if (string.IsNullOrEmpty(wwwCrossPlatform.error))
		{
			listDataExCrossPlatform = DataHelper.BytesToObject<ZtBuffServerListDataEx>(wwwCrossPlatform.bytes, 0, wwwCrossPlatform.bytes.Length);
		}
		Global.Data.ServerData.RecordServerInfos = listDataEx.ListServerData;
		if (listDataExCrossPlatform != null)
		{
			if (Global.Data.ServerData.RecordServerInfos != null)
			{
				if (listDataExCrossPlatform.ListServerData != null)
				{
					Global.Data.ServerData.RecordServerInfos.InsertRange(0, listDataExCrossPlatform.ListServerData);
				}
			}
			else
			{
				Global.Data.ServerData.RecordServerInfos = listDataExCrossPlatform.ListServerData;
			}
		}
		this.RefreshRecordServerItems(Global.Data.ServerData.RecordServerInfos);
		this.m_ListRecordServerItem.repositionNow = true;
		if (Global.Data.ServerData.RecordServerInfos != null && Global.Data.ServerData.RecordServerInfos.Count > 0)
		{
			Global.Data.ServerData.LastServer = Global.Data.ServerData.RecordServerInfos[0];
		}
		wwwCrossPlatform.Dispose();
		wwwCrossPlatform = null;
		www.Dispose();
		www = null;
		Super.HideNetWaiting();
		yield break;
	}

	public void initDataFromServer(XuanFuServerData serverData)
	{
		this.RefreshRecordServerItems(serverData.RecordServerInfos);
		this.RefreshRecommendServerItems(serverData.RecommendServerInfos);
		this.IsFirstRender = true;
	}

	private new void Update()
	{
		if (this.IsFirstRender)
		{
			if (this.m_ListRecordServerItem != null && this.m_ListRecommendServerItem != null)
			{
				this.m_ListRecordServerItem.repositionNow = true;
				this.m_ListRecommendServerItem.repositionNow = true;
			}
			this.IsFirstRender = false;
		}
	}

	private void RefreshRecordServerItems(List<ZtBuffServerInfo> listServerData)
	{
		this.m_ListRecordServerItem.Clear();
		List<XuanFuServerItemFirst> list = new List<XuanFuServerItemFirst>();
		int count = listServerData.Count;
		for (int i = 0; i < count; i++)
		{
			XuanFuServerItemFirst xuanFuServerItemFirst = U3DUtils.NEW<XuanFuServerItemFirst>();
			xuanFuServerItemFirst.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				ZtBuffServerInfo ztBuffServerInfo = (ZtBuffServerInfo)e.Data;
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 2,
					Data = e.Data
				});
			};
			xuanFuServerItemFirst.RefreshServerInfo(listServerData[i]);
			U3DUtils.AddChild(this.m_ListRecordServerItem.gameObject, xuanFuServerItemFirst.gameObject, true);
			list.Add(xuanFuServerItemFirst);
		}
	}

	private void RefreshRecommendServerItems(List<ZtBuffServerInfo> listServerData)
	{
		this.m_ListRecommendServerItem.Clear();
		int num = listServerData.Count;
		if (num > 3)
		{
			num = 3;
		}
		for (int i = 0; i < num; i++)
		{
			XuanFuServerRecommendItemFirst xuanFuServerRecommendItemFirst = U3DUtils.NEW<XuanFuServerRecommendItemFirst>();
			ZtBuffServerInfo serverInfoVO = listServerData[i];
			xuanFuServerRecommendItemFirst.RefreshUI(serverInfoVO, i);
			xuanFuServerRecommendItemFirst.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				ZtBuffServerInfo ztBuffServerInfo = (ZtBuffServerInfo)e.Data;
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 2,
					Data = e.Data
				});
			};
			U3DUtils.AddChild(this.m_ListRecommendServerItem.gameObject, xuanFuServerRecommendItemFirst.gameObject, true);
		}
		this.m_ListRecommendServerItem.repositionNow = true;
	}

	public GButton m_BtnClose;

	public GButton m_BtnAllServer;

	public GButton m_BtnRefreshRecord;

	public UITable m_ListRecordServerItem;

	public UITable m_ListRecommendServerItem;

	private bool IsFirstRender;

	public DPSelectedItemEventHandler DPSelectedItem;

	public UILabel m_BtnAllServerLabel;

	public UILabel m_BtnRefreshRecordLabel;

	public UILabel m_LabZhuTiFuTiShi;

	private DispatcherTimer UITimer;

	public static int RemainRefreshTime;
}
