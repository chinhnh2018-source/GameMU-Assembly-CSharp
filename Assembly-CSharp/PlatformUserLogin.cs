using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Protocol;
using HSGameEngine.GameEngine.Network.Tools;
using HSGameEngine.GameEngine.Scene;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class PlatformUserLogin : UserControl
{
	private void InitTextInPrefabs()
	{
		this.GongGaoBtn.gameObject.SetActive(false);
		this.ClearResBtn.gameObject.SetActive(false);
		this.TextChangeServerHint.Text = Global.GetLang("点击换区");
		this.TextDescription.Text = Global.GetLang("抵制不良游戏 拒绝盗版游戏 注意自我保护 谨防受骗上当 适度游戏益脑 沉迷游戏伤身 合理安排时间 享受健康生活");
		this.TextServerRegion.Text = Global.GetLang("无");
		this.TextServerName.Text = Global.GetLang("无");
		this.EnterGameBtn.Text = Global.GetLang("进入游戏");
		this.ChangeIDBtn.Text = Global.GetLang("切换账号");
		this.TextServerName.transform.localScale = new Vector3(25f, 25f, 1f);
		NGUITools.SetActive(this.LoginBak.gameObject, false);
	}

	private IEnumerator GetGongGaoAddress()
	{
		string url = Global.WebPath("GongGaoAdd.xml");
		WWW www = new WWW(url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			GError.AddErrMsg(string.Format("{0}", Global.GetLang("因为网络原因下载公告配置文件失败...")));
			yield break;
		}
		string content = Global.GetUTF8StringFromBytes(www.bytes);
		XElement xml = XElement.Parse(content);
		List<XElement> xmlList = Global.GetXElementList(xml, "GongGaoAdd");
		for (int i = 0; i < xmlList.Count; i++)
		{
			if ("GongGaoAdd_Android" == Global.GetXElementAttributeStr(xmlList[i], "PingTai"))
			{
				this.gongGaoAddress = Global.GetXElementAttributeStr(xmlList[i], "Add");
				this.gongGaoOpen = Global.GetXElementAttributeInt(xmlList[i], "Open");
			}
		}
		if (Global.IsTuiGuangFenBao)
		{
			this.GongGaoBtn.gameObject.SetActive(this.gongGaoOpen == 1);
			this.ClearResBtn.gameObject.SetActive(this.gongGaoOpen == 1);
		}
		this.isGongGaoLoadFinish = true;
		yield break;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		base.StartCoroutine<bool>(this.GetGongGaoAddress());
		this.LogoTexture.alpha = 0f;
		this.InitTextInPrefabs();
		this.EnterGameBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.EnterGameBtn_MouseLeftButtonUp);
		this.ChangeIDBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string text = Global.StringReplaceAll(Super.GetXapParamByName("uid", string.Empty), ":", string.Empty);
			PlatSDKMgr.ReLogin();
		};
		this.ClearResBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string lang = Global.GetLang("确定清理所有缓存资源，清理后进入游戏时会重新下载，确认要执行清理操作？");
			GChildWindow messageBoxWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), lang, new Vector3(-143f, 35.4f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), null, null, 0, default(Vector3), null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					string[] array = new string[]
					{
						"index.xml",
						"version.xml",
						"HaveDownLoadInfos.txt",
						"NotInPackageAssestDwonLoadMsg.xml",
						"Error.xml",
						"NotInPackage.xml",
						"NotInPackage_TuiGuang.xml",
						"NotInPackage_Map.xml",
						"DownloadFinish.txt",
						"index_TuiGuang.xml"
					};
					BackStageDownloadManager.instance.DeleteFinishDownloadFlag();
					for (int i = 0; i < array.Length; i++)
					{
						string persistentPath = PathUtils.GetPersistentPath(array[i]);
						if (File.Exists(persistentPath))
						{
							File.Delete(persistentPath);
						}
					}
					string lang2 = Global.GetLang("请退出游戏重新进入下载资源");
					GChildWindow gchildWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 0, Global.GetLang("提示"), lang2, new Vector3(-138f, 10f, -0.01f), new Vector3(0f, -55f, -0.01f), Vector3.one, null, null, 0, default(Vector3), null);
					gchildWindow.ChildWindowClose = delegate(object s2, EventArgs e2)
					{
						Application.Quit();
						return true;
					};
				}
				return true;
			};
		};
		this.GongGaoBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.isGongGaoLoadFinish)
			{
				return;
			}
			this.m_GonggaoWindow = U3DUtils.NEW<GChildWindow>();
			this.m_GonggaoWindow.transform.parent = this.Root.parent;
			this.m_GonggaoWindow.transform.localPosition = new Vector3(0f, 0f, -4f);
			this.m_GonggaoWindow.transform.localScale = new Vector3(1f, 1f, 1f);
			this.m_GonggaoWindow.ModalType = ChildWindowModalType.TransBak;
			this.m_GonggaoPart = U3DUtils.NEW<WebGongGao>();
			this.m_GonggaoPart.transform.parent = this.m_GonggaoWindow.Body.transform;
			this.m_GonggaoPart.transform.localPosition = new Vector3(0f, 0f, 0f);
			this.m_GonggaoPart.transform.localScale = new Vector3(1f, 1f, 1f);
			this.m_GonggaoPart.InitGongGaoAddress(this.gongGaoAddress);
			this.m_GonggaoPart.DPSelectedItem = delegate(object a, DPSelectedItemEventArgs d)
			{
				if (d.IDType == -10)
				{
					Object.Destroy(this.m_GonggaoPart.gameObject);
					Object.Destroy(this.m_GonggaoWindow.gameObject);
					this.m_GonggaoWindow = null;
					this.m_GonggaoPart = null;
				}
			};
		};
		UIEventListener.Get(this.SpriteChangeServer.gameObject).onClick = delegate(GameObject s)
		{
			base.StartCoroutine<bool>(this.ShowXuanFuFirstPart(Super.MainWindowRoot));
			if (Global.Data.ServerData != null && Global.Data.ServerData.RecommendServer != null && Global.Data.ServerData.RecommendServer.nOnlineNum > 2)
			{
				base.StartCoroutine<bool>(this.GetData(false));
			}
		};
		this.UserControl_Loaded();
		base.StartCoroutine<bool>(this.GetData(true));
		if (this.Is3DBackground)
		{
			base.StartCoroutine<bool>(this.Init3DMap());
		}
		this.TextServerRegion.Text = string.Empty;
		this.TextVersion.text = string.Format(Global.GetLang("version:{0}_{1}_{2}"), PlatSDKMgr.Version, Context.MainExeVer, Context.ResSwfVer);
		PlatformUserLogin.TextKeyStr = this.TextKey.Text;
	}

	public void ShowYongHuXieYi()
	{
	}

	private void EnterGameBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.CurServerInfoVO == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"CurServerInfoVO == null"
			});
			return;
		}
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"CurServerInfoVO:",
				this.CurServerInfoVO.strURL,
				"__",
				this.CurServerInfoVO.nServerPort
			})
		});
		PlatformUserLogin.GamePingTaiID = this.CurServerInfoVO.nFirstLevelServerID.ToString();
		if (this.CurServerInfoVO.nFirstLevelServerID != PlatformUserLogin.LOGIN_FIRST_ID)
		{
			PlatformUserLogin.LOGIN_FIRST_ID = this.CurServerInfoVO.nFirstLevelServerID;
			LoadConfig.ClearConfigData();
			Super.InitDonwloadConfig();
		}
		GChat.ChatReceiveID = 0;
		Global.ClearJieRiHuoDongConfig();
		if (ConfigTasks.TaskXmlNodeDict.Count <= 0 || ConfigGoods.GoodsXmlNodeDict.Count <= 0 || ConfigNPCs.NPCVODict.Count <= 0 || ConfigMonsters.MonsterXmlNode.Count <= 0)
		{
			if (this.UITimer == null)
			{
				this.UITimer = new DispatcherTimer("platformUserLogin_wait_for_data_UITimer")
				{
					Interval = TimeSpan.FromMilliseconds(500.0)
				};
				this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
				this.UITimer.Start();
				Super.ShowNetWaiting(null);
			}
			return;
		}
		this.EnterGameBtn_MouseLeftButtonUpReal(sender, e);
	}

	private void StopUITimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Stop();
			this.UITimer.Tick = null;
			this.UITimer = null;
		}
	}

	private void UITimer_Tick(object sender, object e)
	{
		MUDebug.Log<string>(new string[]
		{
			"UITimer_Tick"
		});
		if (ConfigTasks.TaskXmlNodeDict.Count > 0 && ConfigGoods.GoodsXmlNodeDict.Count > 0 && ConfigNPCs.NPCVODict.Count > 0 && ConfigMonsters.MonsterXmlNode.Count > 0)
		{
			this.StopUITimer();
			Super.HideNetWaiting();
			this.EnterGameBtn_MouseLeftButtonUpReal(null, null);
		}
	}

	private void EnterGameBtn_MouseLeftButtonUpReal(object sender, MouseEvent e)
	{
		MUDebug.Log<string>(new string[]
		{
			"EnterGameBtn_MouseLeftButtonUpReal"
		});
		if (Global.FilterFieldsDict == null)
		{
			Global.InitFilterFields();
		}
		string text = Global.StringReplaceAll(Super.GetXapParamByName("uid", string.Empty), ":", string.Empty);
		if ("-1" == text)
		{
			PlatSDKMgr.Login(null, string.Empty);
			return;
		}
		if (!YongHuXieYi.IsHaveConfirm() && !Context.IsHaiwai)
		{
			this.ShowYongHuXieYi();
			return;
		}
		string loginMode = Global.GetLoginMode();
		if ("0" != loginMode && this.CurServerInfoVO != null)
		{
			ZtBuffServerInfo curServerInfoVO = this.CurServerInfoVO;
			if (curServerInfoVO.nStatus == 1)
			{
				base.StartCoroutine<bool>(this.GetData(true));
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("该服正在维护中，请耐心等待或选择其它服务器"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					return true;
				};
				return;
			}
			Global.RootParams["serverip"] = curServerInfoVO.strURL;
			Global.RootParams["gameport"] = string.Empty + curServerInfoVO.nServerPort;
			Global.RootParams["loginport"] = string.Empty + curServerInfoVO.nServerPort;
			if (Global.Data.ServerData != null)
			{
				Global.Data.ServerData.LastServer = curServerInfoVO;
			}
			PlayerPrefs.SetInt("NewLastServerInfoID", curServerInfoVO.nServerID);
			PlayerPrefs.SetInt("NewLastServerFirstID", curServerInfoVO.nFirstLevelServerID);
			PlatformUserLogin.RecordSelectServerID = curServerInfoVO.nServerID;
			Global.CurrentZtServerInfo = curServerInfoVO;
			this.ConnectToLoginServer();
		}
	}

	public static void RecordLoginServerIDs(ZtBuffServerInfo infoVo)
	{
		string text = Global.StringReplaceAll(Super.GetXapParamByName("uid", string.Empty), ":", string.Empty);
		if ("-1" == text)
		{
			MUDebug.LogError<string>(new string[]
			{
				"平台ID为-1"
			});
		}
		if (string.IsNullOrEmpty(text) || text == "-1")
		{
			text = "LastServerInfoID";
		}
		string text2 = PlayerPrefs.GetString(text);
		if (string.IsNullOrEmpty(text2))
		{
			int @int = PlayerPrefs.GetInt("LastServerInfoID");
			if (@int > 0)
			{
				text2 = @int.ToString();
			}
		}
		bool flag = false;
		if (string.IsNullOrEmpty(text2) || PlayerPrefs.GetInt("IsSendedToServer") != 1)
		{
			flag = true;
		}
		else
		{
			string[] array = text2.Split(new char[]
			{
				','
			});
			if (array.Length > 0 && !string.IsNullOrEmpty(array[0]))
			{
				if (array[0].Split(new char[]
				{
					'|'
				})[0] != infoVo.nServerID.ToString())
				{
					flag = true;
				}
				else if (array[0].Split(new char[]
				{
					'|'
				})[1] != infoVo.nFirstLevelServerID.ToString())
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			string text3 = Global.ServerListURLSecond + "WriteUserLogInServerId.aspx";
			ClientServerListDataEx clientServerListDataEx = new ClientServerListDataEx();
			clientServerListDataEx.Time = TimeManager.GetCorrectLocalTime();
			clientServerListDataEx.Md5 = MD5Helper.get_md5_string(PlatformUserLogin.TextKeyStr + clientServerListDataEx.Time.ToString());
			clientServerListDataEx.ServerId = string.Format("{0}|{1}|{2}", infoVo.nServerID, infoVo.nFirstLevelServerID, infoVo.nSecondLevelServerID);
			clientServerListDataEx.UserId = text;
			byte[] array2 = DataHelper.ObjectToBytes<ClientServerListDataEx>(clientServerListDataEx);
			WWW www = new WWW(text3, array2);
			PlayerPrefs.SetInt("IsSendedToServer", 1);
		}
		string orderServerIDs = PlatformUserLogin.GetOrderServerIDs(text2, infoVo);
		PlayerPrefs.SetString(text, orderServerIDs);
		PlayerPrefs.SetString("LastServerInfoID", orderServerIDs);
		PlayerPrefs.SetInt("NewLastServerInfoID", infoVo.nServerID);
		PlayerPrefs.SetInt("NewLastServerFirstID", infoVo.nFirstLevelServerID);
	}

	private static string GetOrderServerIDs(string serverIDs, ZtBuffServerInfo infoVo)
	{
		int nServerID = infoVo.nServerID;
		string empty = string.Empty;
		string[] array = serverIDs.Split(new char[]
		{
			','
		});
		ArrayList arrayList = new ArrayList();
		arrayList.AddRange(array);
		int num = -1;
		int count = arrayList.Count;
		for (int i = 0; i < count; i++)
		{
			string[] array2 = arrayList[i].ToString().Split(new char[]
			{
				'|'
			});
			if (array2.Length == 1)
			{
				if (nServerID == ConvertExt.SafeConvertToInt32(array2[0]) && infoVo.nFirstLevelServerID == 10 && infoVo.nSecondLevelServerID == 100)
				{
					num = i;
					break;
				}
			}
			else if (array2.Length == 3 && nServerID == ConvertExt.SafeConvertToInt32(array2[0]) && infoVo.nFirstLevelServerID == ConvertExt.SafeConvertToInt32(array2[1]) && infoVo.nSecondLevelServerID == ConvertExt.SafeConvertToInt32(array2[2]))
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			arrayList.RemoveAt(num);
		}
		string text = string.Format("{0}|{1}|{2}", nServerID, infoVo.nFirstLevelServerID, infoVo.nSecondLevelServerID);
		arrayList.Insert(0, text);
		int num2 = arrayList.Count;
		if (num2 > 5)
		{
			num2 = 5;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int j = 0; j < num2; j++)
		{
			stringBuilder.Append(arrayList[j]);
			if (j != num2 - 1)
			{
				stringBuilder.Append(',');
			}
		}
		return stringBuilder.ToString();
	}

	public override void Destroy()
	{
		base.Destroy();
		if (YongHuXieYi.xmlEle != null)
		{
			YongHuXieYi.xmlEle = null;
		}
		if (null == Global.RoleSel3DBakMapLoader && Global.RoleSel3DBakMapWWW != null)
		{
			Global.RoleSel3DBakMapWWW = null;
		}
		if (null == Global.RoleCreate3DBakMapLoader && Global.RoleCreate3DBakMapWWW != null)
		{
			Global.RoleCreate3DBakMapWWW = null;
		}
	}

	private void ShowXuanFuPart(Canvas root)
	{
		XuanFuPart xuanFuPart = U3DUtils.NEW<XuanFuPart>();
		xuanFuPart.ServerData = Global.Data.ServerData;
		xuanFuPart.initDataFromServer();
		xuanFuPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == 1)
			{
				Object.Destroy((s as XuanFuPart).gameObject);
			}
			else if (e.ID == 2)
			{
				ZtBuffServerInfo curServerInfo = (ZtBuffServerInfo)e.Data;
				this.SetCurServerInfo(curServerInfo);
				Object.Destroy((s as XuanFuPart).gameObject);
			}
			else if (e.ID == 3)
			{
				base.StartCoroutine<bool>(this.ShowXuanFuFirstPart(Super.MainWindowRoot));
				Object.Destroy((s as XuanFuPart).gameObject);
			}
		};
		root.Children.Add(xuanFuPart);
	}

	public IEnumerator ShowXuanFuFirstPart(Canvas root)
	{
		if (Global.Data.ServerData == null)
		{
			yield return null;
		}
		if (Global.Data.ServerData != null)
		{
			XuanFuPartFirst xuanFuPartFirst = U3DUtils.NEW<XuanFuPartFirst>();
			xuanFuPartFirst.initDataFromServer(Global.Data.ServerData);
			xuanFuPartFirst.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 1)
				{
					Object.Destroy((s as XuanFuPartFirst).gameObject);
				}
				else if (e.ID == 2)
				{
					ZtBuffServerInfo curServerInfo = (ZtBuffServerInfo)e.Data;
					this.SetCurServerInfo(curServerInfo);
					Object.Destroy((s as XuanFuPartFirst).gameObject);
				}
				else if (e.ID == 3)
				{
					base.StartCoroutine<bool>(this.GetAllData(s as XuanFuPartFirst));
				}
			};
			root.Children.Add(xuanFuPartFirst);
		}
		yield break;
	}

	public IEnumerator ShowXuanFuFirstPartEx(Canvas root)
	{
		Global.Data.ServerData = null;
		this.GetData(false);
		if (Global.Data.ServerData == null)
		{
			yield return null;
		}
		this.ShowXuanFuFirstPart(root);
		yield break;
	}

	public IEnumerator GetData(bool isRefreshUI = true)
	{
		Super.ShowNetWaiting(null);
		string url = Global.ServerListURL + "GetUserServerListZt.aspx";
		string strUID = Global.StringReplaceAll(Super.GetXapParamByName("uid", string.Empty), ":", string.Empty);
		if (string.IsNullOrEmpty(strUID) || strUID == "-1")
		{
			strUID = "LastServerInfoID";
		}
		string serverIDs = PlayerPrefs.GetString(strUID);
		if (string.IsNullOrEmpty(serverIDs))
		{
			int recordServerID = PlayerPrefs.GetInt("LastServerInfoID");
			if (recordServerID > 0)
			{
				serverIDs = recordServerID.ToString();
			}
		}
		WWWForm wwwForm = new WWWForm();
		wwwForm.AddField("serverId", serverIDs);
		WWW www = new WWW(url, wwwForm);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Super.HideNetWaiting();
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接(GetUserServerList),请检查您的网络是否正常！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				Application.Quit();
				return true;
			};
			yield break;
		}
		ZtBuffServerListDataEx listDataEx = DataHelper.BytesToObject<ZtBuffServerListDataEx>(www.bytes, 0, www.bytes.Length);
		if (listDataEx == null)
		{
			Super.HideNetWaiting();
			MUDebug.LogError<string>(new string[]
			{
				"没有找到远程的ServerList.xml"
			});
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
		Global.Data.ServerData = new XuanFuServerData();
		Global.Data.ServerData.RecordServerInfos = listDataEx.ListServerData;
		Global.Data.ServerData.RecommendServerInfos = listDataEx.RecommendListServerData;
		if (listDataExCrossPlatform != null)
		{
			if (Global.Data.ServerData.RecommendServerInfos != null)
			{
				if (listDataExCrossPlatform.RecommendListServerData != null)
				{
					Global.Data.ServerData.RecommendServerInfos.InsertRange(0, listDataExCrossPlatform.RecommendListServerData);
				}
			}
			else
			{
				Global.Data.ServerData.RecommendServerInfos = listDataExCrossPlatform.RecommendListServerData;
			}
		}
		if (Global.Data.ServerData.RecommendServerInfos != null && Global.Data.ServerData.RecommendServerInfos.Count > 0)
		{
			Global.Data.ServerData.RecommendServer = Global.Data.ServerData.RecommendServerInfos[0];
		}
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
		if (Global.Data.ServerData.RecordServerInfos != null && Global.Data.ServerData.RecordServerInfos.Count > 0)
		{
			bool hasLastServer = false;
			int lastServerId = PlayerPrefs.GetInt("NewLastServerInfoID");
			int lastFirstId = PlayerPrefs.GetInt("NewLastServerFirstID");
			if (lastServerId != 0)
			{
				int recordCount = Global.Data.ServerData.RecordServerInfos.Count;
				for (int i = 0; i < recordCount; i++)
				{
					if (Global.Data.ServerData.RecordServerInfos[i].nServerID == lastServerId && Global.Data.ServerData.RecordServerInfos[i].nFirstLevelServerID == lastFirstId)
					{
						Global.Data.ServerData.LastServer = Global.Data.ServerData.RecordServerInfos[i];
						hasLastServer = true;
						break;
					}
				}
			}
			if (!hasLastServer)
			{
				Global.Data.ServerData.LastServer = Global.Data.ServerData.RecordServerInfos[0];
			}
		}
		else if (Global.Data.ServerData.RecommendServerInfos != null && Global.Data.ServerData.RecommendServerInfos.Count > 0)
		{
			Global.Data.ServerData.LastServer = Global.Data.ServerData.RecommendServerInfos[0];
		}
		Global.Data.ServerData.ClientInfo = listDataEx.ClientInfo;
		MUDebug.Log<string>(new string[]
		{
			"listDataEx.ClientInfo:" + listDataEx.ClientInfo
		});
		if (isRefreshUI)
		{
			this.RefreshByServerData();
		}
		Super.HideNetWaiting();
		www.Dispose();
		www = null;
		yield break;
	}

	private IEnumerator GetAllData(XuanFuPartFirst xuanFuPartFirst)
	{
		Super.ShowNetWaiting(null);
		string url = Global.ServerListURL + "GetServerListZt.aspx";
		MUDebug.Log<string>(new string[]
		{
			"ServerListURL GetServerList:" + url
		});
		ZtClientServerListData clientListData = new ZtClientServerListData();
		string strUID = Global.StringReplaceAll(Super.GetXapParamByName("uid", string.Empty), ":", string.Empty);
		clientListData.strUID = strUID;
		clientListData.lTime = TimeManager.GetCorrectLocalTime();
		clientListData.strMD5 = MD5Helper.get_md5_string("HWjKO26fEJvZ27f8v0Qu9EGZ3k3phFO4NCt8A" + clientListData.lTime.ToString());
		byte[] clientBytes = DataHelper.ObjectToBytes<ZtClientServerListData>(clientListData);
		WWW www = new WWW(url, clientBytes);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Super.HideNetWaiting();
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接(GetServerList),请检查您的网络是否正常！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				Application.Quit();
				return true;
			};
			yield break;
		}
		ZtBuffServerListData listDataEx = DataHelper.BytesToObject<ZtBuffServerListData>(www.bytes, 0, www.bytes.Length);
		if (listDataEx != null)
		{
			MUDebug.Log<string>(new string[]
			{
				"listDataEx" + listDataEx.listServerData.Count
			});
		}
		url = Global.ServerListCrossPlatfomURL + "GetServerListZt.aspx";
		MUDebug.Log<string>(new string[]
		{
			"ServerListCrossPlatfomURL :" + url
		});
		WWW wwwCrossPlatform = new WWW(url, clientBytes);
		yield return wwwCrossPlatform;
		if (string.IsNullOrEmpty(wwwCrossPlatform.error))
		{
			ZtBuffServerListData listDataExCrossPlatform = DataHelper.BytesToObject<ZtBuffServerListData>(wwwCrossPlatform.bytes, 0, wwwCrossPlatform.bytes.Length);
			if (listDataExCrossPlatform != null)
			{
				MUDebug.Log<string>(new string[]
				{
					"listDataExCrossPlatform" + listDataExCrossPlatform.listServerData.Count
				});
				listDataEx.listServerData.InsertRange(0, listDataExCrossPlatform.listServerData);
			}
		}
		if (!listDataEx.IsAllPause)
		{
			if (listDataEx.listServerData.Count == 0)
			{
				Super.HideNetWaiting();
				MUDebug.LogError<string>(new string[]
				{
					"没有找到远程的ServerList.xml"
				});
				yield break;
			}
			this.ResetServerData(listDataEx);
			Global.Data.ServerData.ServerListData = listDataEx;
			if (xuanFuPartFirst != null)
			{
				Object.Destroy(xuanFuPartFirst.gameObject);
			}
			Super.HideNetWaiting();
			this.ShowXuanFuPart(Super.MainWindowRoot);
		}
		else
		{
			Super.HideNetWaiting();
			GChildWindow messageBoxWindow2 = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), string.Format(Global.GetLang("维护中，预计维护结束时间：\n {0} ！"), listDataEx.strMaintainTerminalTime), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow2.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow2.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow2);
				return true;
			};
		}
		wwwCrossPlatform.Dispose();
		wwwCrossPlatform = null;
		www.Dispose();
		www = null;
		yield break;
	}

	private void ResetServerData(ZtBuffServerListData listDataEx)
	{
		try
		{
			int count = listDataEx.listServerData.Count;
			for (int i = 0; i < count; i++)
			{
				if (listDataEx.listServerData[i].nFirstLevelServerID == 10 && listDataEx.listServerData[i].listServerData.Count == 1)
				{
					List<SecondLevelServerListData> listServerData = this.ResetServerInfoData(listDataEx.listServerData[i].listServerData[0].listServerData);
					listDataEx.listServerData[i].listServerData = listServerData;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private List<SecondLevelServerListData> ResetServerInfoData(List<ZtBuffServerInfo> ServerInfos)
	{
		ServerInfos.Reverse();
		List<SecondLevelServerListData> list = new List<SecondLevelServerListData>();
		if (ServerInfos == null)
		{
			return list;
		}
		int num = 50;
		int count = ServerInfos.Count;
		int num2;
		if (count % num == 0)
		{
			num2 = count / num;
		}
		else
		{
			num2 = count / num + 1;
		}
		int num3 = count % num;
		for (int i = num2 - 1; i >= 0; i--)
		{
			SecondLevelServerListData secondLevelServerListData = new SecondLevelServerListData();
			int num4;
			int num5;
			if (i < num2 - 1)
			{
				num4 = i * num;
				num5 = i * num + num - 1;
				secondLevelServerListData.strSecondtLevelServerName = string.Format(Global.GetLang("{0}-{1}区"), num4 + 1, num5 + 1);
			}
			else
			{
				num4 = i * num;
				if (num3 != 0)
				{
					num5 = i * num + num3 - 1;
				}
				else
				{
					num5 = i * num + num - 1;
				}
				secondLevelServerListData.strSecondtLevelServerName = string.Format(Global.GetLang("{0}-{1}区"), num4 + 1, num5 + 1);
			}
			List<ZtBuffServerInfo> list2 = new List<ZtBuffServerInfo>();
			for (int j = num5; j >= num4; j--)
			{
				list2.Add(ServerInfos[j]);
			}
			secondLevelServerListData.nSecondLevelServerID = 100;
			secondLevelServerListData.listServerData = list2;
			list.Add(secondLevelServerListData);
		}
		return list;
	}

	private void RefreshByServerData()
	{
		if (Global.Data.ServerData.LastServer != null)
		{
			this.SetCurServerInfo(Global.Data.ServerData.LastServer);
		}
		else
		{
			this.SetCurServerInfo(Global.Data.ServerData.RecommendServer);
		}
	}

	public void SetCurServerInfo(ZtBuffServerInfo serverVO)
	{
		if (serverVO != null)
		{
			this.CurServerInfoVO = serverVO;
			if (serverVO.nFirstLevelServerID != 10)
			{
				this.TextServerName.text = serverVO.strServerName + Global.GetLang("区");
			}
			else
			{
				this.TextServerName.text = serverVO.strServerName;
			}
			if (serverVO.nStatus == 1)
			{
				this.WeiHuContainer.gameObject.SetActive(true);
				this.TextTitle.text = serverVO.strMaintainTxt;
				this.TextStartTime.text = string.Format(Global.GetLang("维护开始时间:{0}"), serverVO.strMaintainStarTime);
				this.TextEndTime.text = string.Format(Global.GetLang("维护结束时间:{0}"), serverVO.strMaintainTerminalTime);
			}
			else
			{
				this.WeiHuContainer.gameObject.SetActive(false);
			}
		}
	}

	private IEnumerator Init3DMap()
	{
		bool initScene = false;
		AssetBundle CurrentMapLoader = null;
		if (null == Global.Login3DBakMapLoader)
		{
			Super.ShowNetWaiting(Global.GetLang("正在读取资源..."));
			Global.MainCamera.backgroundColor = Color.black;
			WWW www = null;
			if (Global.IsTuiGuangFenBao)
			{
				www = new WWW(PathUtils.WebPath("Map/denglu_TG.unity3d"));
			}
			else
			{
				www = new WWW(PathUtils.WebPath("Map/Denglu.unity3d"));
			}
			yield return www;
			if (www.error != null)
			{
				yield break;
			}
			CurrentMapLoader = www.assetBundle;
			Global.Login3DBakMapLoader = CurrentMapLoader;
			initScene = true;
		}
		else
		{
			CurrentMapLoader = Global.Login3DBakMapLoader;
		}
		if (initScene)
		{
			Global.AudioListener43D.enabled = false;
			Global.MainCamera.transform.localPosition = new Vector3(8f, 2f, 98f);
			Global.MainCamera.transform.localRotation = Quaternion.Euler(0.597f, 181.89f, -0.6f);
			Global.MainCamera.farClipPlane = 2000f;
			if (Global.IsTuiGuangFenBao)
			{
				Global.MainCamera.fieldOfView = 21f;
			}
			else
			{
				Global.MainCamera.fieldOfView = 35f;
			}
			Global.MainCamera.backgroundColor = Color.black;
			LoadingMap.ClearSpeicalMapEffect();
			PerformanceCtrl.ResetNormalSceneSettings();
			LayerCullDistanceslMgr.SetCameraLayerDistance(Global.MainCamera, 1000f);
			string levelName = null;
			if (Global.IsTuiGuangFenBao)
			{
				levelName = "denglu_TG";
			}
			else
			{
				levelName = "denglu";
			}
			AsyncOperation asyncOperation = Application.LoadLevelAsync(levelName);
			Global.DirectLight.enabled = false;
			Global.IsInGameScene = false;
			yield return asyncOperation;
			PerformanceCtrl.CopyCameraParmasForDengLu();
			Super.HideNetWaiting();
		}
		string skeletonName = null;
		skeletonName = Global.GetSkeletonNameByOccupation(0);
		U3DUtils.LoadSkeletonByName(skeletonName, true);
		skeletonName = Global.GetSkeletonNameByOccupation(1);
		U3DUtils.LoadSkeletonByName(skeletonName, true);
		skeletonName = Global.GetSkeletonNameByOccupation(2);
		U3DUtils.LoadSkeletonByName(skeletonName, true);
		skeletonName = Global.GetSkeletonNameByOccupation(3);
		U3DUtils.LoadSkeletonByName(skeletonName, true);
		skeletonName = Global.GetSkeletonNameByOccupation(5);
		U3DUtils.LoadSkeletonByName(skeletonName, true);
		if (Global.RoleCreate3DBakMapLoader == null)
		{
			WWW www2 = new WWW(PathUtils.WebPath("Map/chuangjue.unity3d"));
			Global.RoleCreate3DBakMapWWW = www2;
			yield return www2;
			if (www2.error != null)
			{
				yield break;
			}
			Global.RoleCreate3DBakMapLoader = www2.assetBundle;
		}
		if (Global.RoleSel3DBakMapLoader == null)
		{
			WWW www3 = new WWW(PathUtils.WebPath("Map/xuanjue.unity3d"));
			Global.RoleSel3DBakMapWWW = www3;
			yield return www3;
			if (www3.error != null)
			{
				yield break;
			}
			Global.RoleSel3DBakMapLoader = www3.assetBundle;
		}
		yield break;
	}

	public void DestroyLogin3Map()
	{
		if (null != Global.Login3DBakMapLoader)
		{
			Global.Login3DBakMapLoader.Unload(true);
			Global.Login3DBakMapLoader = null;
			GameObject gameObject = GameObject.FindWithTag("MainCamera");
			if (null != gameObject)
			{
				Global.DirectLight.enabled = true;
				Global.MainCamera.transform.localPosition = Vector3.zero;
				Global.MainCamera.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
				Global.MainCamera.far = 35f;
				Global.MainCamera.fieldOfView = 30f;
				Global.MainCamera.backgroundColor = Color.black;
				RenderSettings.ambientLight = new Color(1f, 1f, 1f);
				Global.AudioListener43D.enabled = false;
				Global.AudioListener4UI.enabled = true;
				Global.BackgroundAudio4UI.PlayAudio("Audio/Map/RoleManager.mp3", true, false);
			}
		}
	}

	public void ConnectToLoginServer()
	{
		this.ResetTCPClient();
		this.UserControl_Loaded();
		string xapParamByName = Super.GetXapParamByName("serverip", "192.168.0.206");
		this.MyLoginIP = xapParamByName;
		Super.ShowNetWaiting(Global.GetLang("正在连接用户服务器..."));
		this.tcpClient.Connect(xapParamByName, Global.GetUserLoginPort());
	}

	public void ResetTCPClient()
	{
		if (this.tcpClient != null)
		{
			this.tcpClient.Disconnect(2);
			this.tcpClient.Destroy();
			this.tcpClient = new TCPClient(2);
		}
	}

	private void UserControl_Loaded()
	{
		this.tcpClient.SocketConnect += this.SocketConnect;
	}

	private void SocketConnect(object sender, MUSocketConnectEventArgs e)
	{
		MainGame.QueueOnMainThread(delegate()
		{
			switch (e.NetSocketType)
			{
			case 0:
				if (e.Error == "Success")
				{
					string text = string.Empty;
					string text2 = Global.StringReplaceAll(Super.GetXapParamByName("uid", string.Empty), ":", string.Empty);
					string text3 = Global.StringReplaceAll(Super.GetXapParamByName("n", string.Empty), ":", string.Empty);
					string text4 = Global.StringReplaceAll(Super.GetXapParamByName("t", string.Empty), ":", string.Empty);
					string text5 = Global.StringReplaceAll(Super.GetXapParamByName("cm", string.Empty), ":", string.Empty);
					string text6 = Global.StringReplaceAll(Super.GetXapParamByName((!Global.GetNowServerIsZhuTiFu(Global.Data.ServerData.LastServer)) ? "token" : "Zttoken", string.Empty), ":", string.Empty);
					int num = 20;
					text = StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						20140624,
						text2,
						text3,
						text4,
						text5,
						text6
					});
					byte[] bytes = new UTF8Encoding().GetBytes(text);
					MUDebug.Log<string>(new string[]
					{
						string.Concat(new object[]
						{
							"ServerId : ",
							Global.Data.ServerData.LastServer.nServerID,
							"  token : ",
							text6
						})
					});
					TCPOutPacket tcpoutPacket = new TCPOutPacket();
					tcpoutPacket.PacketCmdID = (short)num;
					tcpoutPacket.FinalWriteData(bytes, 0, bytes.Length);
					this.tcpClient.SendData(tcpoutPacket);
				}
				else
				{
					this.ActiveDisconnect = true;
					Super.HideNetWaiting();
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), StringUtil.substitute(Global.GetLang("连接服务器失败，请稍后再试"), new object[0]), -1, -1, -1, -1, false);
					if (this != null && this.gameObject != null && NGUITools.GetActive(this.gameObject))
					{
						this.StartCoroutine<bool>(this.GetData(true));
					}
				}
				break;
			case 1:
				this.ActiveDisconnect = true;
				Super.HideNetWaiting();
				Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), StringUtil.substitute(Global.GetLang("向游戏用户服务器发送信息失败"), new object[0]), -1, -1, -1, -1, false);
				break;
			case 2:
				break;
			case 3:
				GScene.ServerStopGame();
				if (this != null && this.gameObject != null && NGUITools.GetActive(this.gameObject))
				{
					this.StartCoroutine<bool>(this.GetData(true));
				}
				if (!this.ActiveDisconnect)
				{
					Super.HideNetWaiting();
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), StringUtil.substitute(Global.GetLang("与游戏用户服务器的连接被断开"), new object[0]), -1, -1, -1, -1, false);
				}
				break;
			case 4:
				this.ActiveDisconnect = true;
				this.tcpClient.Disconnect(2);
				if ("-1" == e.fields[0])
				{
					Super.HideNetWaiting();
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("您输入的用户名或密码错误, 请重新输入后再试..."), -1, -1, -1, -1, false);
				}
				else if ("-2" == e.fields[0])
				{
					Super.HideNetWaiting();
					Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("登陆用户服务器时失败, 客户端的版本太旧，请更新客户端后再重新登陆"), -1, -1, -1, -1, false);
				}
				else
				{
					GameInstance.Game.CurrentSession.UserID = e.fields[0];
					GameInstance.Game.CurrentSession.UserName = e.fields[1];
					GameInstance.Game.CurrentSession.UserToken = e.fields[2];
					GameInstance.Game.CurrentSession.UserIsAdult = Convert.ToInt32(e.fields[3]);
					Super.HideNetWaiting();
					this.tcpClient.SocketConnect -= this.SocketConnect;
					this.tcpClient.Destroy();
					this.tcpClient = null;
					if (this.LoginGameToLineServer != null)
					{
						this.LoginGameToLineServer.Invoke(this, EventArgs.Empty);
					}
				}
				break;
			default:
				throw new Exception(Global.GetLang("错误的Socket操作类型"));
			}
		});
	}

	public static int LOGIN_FIRST_ID = -1;

	public static string GamePingTaiID = "10";

	public GButton EnterGameBtn;

	public GButton ChangeIDBtn;

	public TextBlock TextServerRegion;

	public TextBlock TextServerName;

	public UISprite SpriteChangeServer;

	public GameObject WeiHuContainer;

	public TextBlock TextStartTime;

	public TextBlock TextEndTime;

	public TextBlock TextTitle;

	public TextBlock TextChangeServerHint;

	public TextBlock TextVersion;

	public TextBlock TextDescription;

	public Transform Root;

	public GButton GongGaoBtn;

	public GButton ClearResBtn;

	public UITexture LogoTexture;

	public TextBlock TextKey;

	public UISprite LoginBak;

	public static int RecordSelectServerID = -1;

	private GChildWindow m_GonggaoWindow;

	private WebGongGao m_GonggaoPart;

	private GChildWindow m_XieYiWindow;

	private YongHuXieYi m_XieYiPart;

	private bool Is3DBackground = true;

	public EventHandler LoginGameToLineServer;

	private ZtBuffServerInfo CurServerInfoVO;

	private TCPClient tcpClient = new TCPClient(2);

	private bool ActiveDisconnect;

	private string MyLoginIP = string.Empty;

	private string gongGaoAddress = string.Empty;

	private int gongGaoOpen = -1;

	private bool isGongGaoLoadFinish;

	protected DispatcherTimer UITimer;

	private static string TextKeyStr = string.Empty;
}
