using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Tools;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using HSGameEngine.JavaPlugins;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class LoadingMap : UserControl
{
	protected override void InitializeComponent()
	{
		this.progressText.text = string.Empty;
		this.Bak.transform.localScale = Super.GetScreenSize();
		this.MapPic.ImageDownloaded = new URLImage.ImageDownloadedEventHandler(this.LoadImageFinish);
		this.MapPic.ImageDownloadedErr = new URLImage.ImageDownloadedEventHandler(this.LoadImageFinish);
		this.LogoPic.ImageDownloaded = new URLImage.ImageDownloadedEventHandler(this.LoadLogoImageFinish);
		this.LogoPic.ImageDownloadedErr = new URLImage.ImageDownloadedEventHandler(this.LoadLogoImageFinish);
		this.LogoPic.gameObject.SetActive(false);
		base.InitializeComponent();
		this.UserControl_Loaded(this);
	}

	public override void Destroy()
	{
		this.StopUITimer();
		base.Destroy();
	}

	public int MapCode
	{
		get
		{
			return this._MapCode;
		}
		set
		{
			this._MapCode = value;
			if (Global.IsTuiGuangFenBao)
			{
				this.LogoPic.gameObject.SetActive(false);
			}
			else
			{
				bool loadingLogoImageVisiable = ConfigSettings.GetLoadingLogoImageVisiable(this._MapCode);
				this.LogoPic.gameObject.SetActive(loadingLogoImageVisiable);
			}
		}
	}

	public int LoadType
	{
		get
		{
			return this._LoadType;
		}
		set
		{
			this._LoadType = value;
			this.InitCtrls();
			if (this.LoadType == 0)
			{
				this.MapPic.URL = ConfigSettings.GetLoadingImage(this.MapCode);
			}
			else if (this.LoadType != 1)
			{
				if (this.LoadType == 2)
				{
					if (Global.IsInZhuTiFuActivity())
					{
						this.MapPic.URL = Global.GetZhuTiFuNetImg("Loading", "LoadGame/11.jpg");
					}
					else
					{
						this.MapPic.URL = ((!Global.IsKuaFuMap(this.MapCode, true)) ? this.GetLoadGamePicName() : ConfigSettings.GetLoadingImage(this.MapCode));
					}
				}
			}
			if (!this.MapPic.URL.Contains("NetImages/Map/"))
			{
				this.LogoPic.gameObject.SetActive(false);
			}
		}
	}

	private string GetLoadGamePicName()
	{
		if (Global.IsTuiGuangFenBao)
		{
			return "LoadGame/Loading_tuiguang2.jpg.qj";
		}
		return "LoadGame/11.jpg";
	}

	public double ProgressShowPercent
	{
		get
		{
			return this.LoadingMapShowPercent;
		}
		set
		{
			this.LoadingMapShowPercent = value;
		}
	}

	public double ProgressPercent
	{
		get
		{
			return this.ProgressCenter.Percent;
		}
		set
		{
			this.ProgressCenter.Percent = value;
		}
	}

	public string ProgressText
	{
		set
		{
			this.progressText.Text = value;
		}
	}

	public string ProgressInfo
	{
		set
		{
			this.progressText2.Text = value;
			if (MUDebug.IsOpenDebug)
			{
				MUDebug.Log<string>(new string[]
				{
					"LoadingMap:" + value
				});
			}
		}
	}

	private void UserControl_Loaded(UserControl sender)
	{
		NGUITools.SetActive(this.WebErrMsg, false);
		if (Super.AutoSystemChatItemsArray != null)
		{
			int num = Random.Range(0, Super.AutoSystemChatItemsArray.Length);
			this.HintUser1.Text = Super.AutoSystemChatItemsArray[num];
		}
		if (ConfigSystemParam.GetSystemParamIntByName("CloseLoadingInfo") == 1L)
		{
			this.progressText2.Visibility = false;
		}
		if (Context.IsHaiwai)
		{
			this.progressText2.Visibility = false;
		}
	}

	private void InitCtrls()
	{
	}

	private void LoadImageFinish(object obj)
	{
		if (null != Global.CurrentTerrainLoader)
		{
			AssetBundle currentTerrainLoader = Global.CurrentTerrainLoader;
			Global.CurrentTerrainLoader = null;
			currentTerrainLoader.Unload(true);
		}
		if (null != Global.CurrentMapLoader)
		{
			AssetBundle currentMapLoader = Global.CurrentMapLoader;
			Global.CurrentMapLoader = null;
			currentMapLoader.Unload(true);
		}
		this.ProgressInfo = "L1";
		base.StartCoroutine<bool>(this.LoadEmptyScene());
	}

	private void LoadLogoImageFinish(object obj)
	{
	}

	private void StartDownloadGameRes()
	{
		Super.HideNetWaiting();
		this.ProgressInfo = "sd1";
		if (Global.IsPopupDownloadMapWindow(this.MapCode) && !Global.IsFuBenFenBaoMap)
		{
			this.ProgressInfo = "sd2";
			string[] array = new string[]
			{
				Global.GetLang("立即下载"),
				Global.GetLang("稍后下载")
			};
			string message = string.Format(Global.GetLang("当前需要下载场景资源【{0}KB】，是否立即下载？"), Global.GetFenBaoMapSize(this.MapCode));
			NoTitleWindow titleWindow = Super.ShowDialogBox(MainGame._current.Stage, 1, message, (int)Global.GlobalMainWindow.ActualWidth / 2 - 150, 100, 0, string.Empty, array[0], array[1]);
			titleWindow.transform.localPosition = new Vector3(titleWindow.transform.localPosition.x, titleWindow.transform.localPosition.y, -1000f);
			MyDialogBoxPart componentInChildren = titleWindow.GetComponentInChildren<MyDialogBoxPart>();
			if (componentInChildren != null)
			{
				componentInChildren.HintText_Label.transform.localPosition = new Vector3(-148f, 36f, -0.1f);
				componentInChildren.HintText_Label.MaxWidth = 318.0;
			}
			titleWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int dialogBoxReturn = (s1 as NoTitleWindow).DialogBoxReturn;
				Super.CloseNoTitleWindow(MainGame._current.Stage, s1 as NoTitleWindow);
				titleWindow = null;
				if (dialogBoxReturn == 0)
				{
					this.ProgressInfo = "sd3";
					this.StartCoroutine<bool>(this.InitGameRes());
				}
				else
				{
					this.ProgressInfo = "sd4";
					this.BackToLogin(3, string.Empty);
				}
				return true;
			};
		}
		else
		{
			base.StartCoroutine<bool>(this.InitGameRes());
			Global.IsFuBenFenBaoMap = false;
			this.ProgressInfo = "sd5";
		}
	}

	private IEnumerator InitGameRes()
	{
		this.ProgressInfo = "g1";
		BackStageDownloadManager.instance.StopDownloadByExternal();
		if (null != Global.BackgroundAudio4UI)
		{
			Global.BackgroundAudio4UI.StopPlay();
		}
		if (null != Global.AudioListener4UI)
		{
			Global.AudioListener4UI.enabled = false;
		}
		if (null != Global.AudioListener43D)
		{
			Global.AudioListener43D.enabled = true;
		}
		this.ProgressInfo = "g2";
		this.StartUITimer();
		string url = string.Empty;
		this.ProgressFormatText = Global.GetLang("正在下载地形配置文件, 请稍候【{0}%】");
		this.ProgressCenter.Percent = 0.0;
		this.progressText.Text = string.Format(this.ProgressFormatText, 0);
		url = Global.WebPath(StringUtil.substitute("MapConfig/MapConfig{0}.unity3d", new object[]
		{
			ConfigSettings.GetMapPicCodeByCode(this.MapCode)
		}));
		this.ProgressInfo = "g3";
		this.www = new WWW(url);
		yield return this.www;
		this.ProgressInfo = "g4";
		if (!string.IsNullOrEmpty(this.www.error))
		{
			this.WebErrMsg.Text = Global.GetLang("因为网络原因加载地形配置文件失败...");
			this.WebErrMsg.Visibility = true;
			MUDebug.LogError<string>(new string[]
			{
				"读取Map地图配置文件失败:" + url
			});
			this.OnFatalError();
			this.ProgressInfo = "g5";
			yield break;
		}
		Global.CurrentMapLoader = this.www.assetBundle;
		this.ProgressInfo = "g6";
		this.ProgressFormatText = Global.GetLang("正在加载地图配置文件, 请稍候【{0}%】");
		this.ProgressCenter.Percent = 0.0;
		this.progressText.Text = string.Format(this.ProgressFormatText, 0);
		url = Global.WebPath(StringUtil.substitute("GameSite/{0}/Map.unity3d", new object[]
		{
			Global.Data.GamePingTaiID
		}));
		this.ProgressInfo = "g7";
		this.www = new WWW(url);
		yield return this.www;
		this.ProgressInfo = "g8";
		if (!string.IsNullOrEmpty(this.www.error))
		{
			this.WebErrMsg.Text = Global.GetLang("因为网络原因加载地图配置文件失败...");
			this.WebErrMsg.Visibility = true;
			MUDebug.LogError<string>(new string[]
			{
				"加载Map信息配置失败:" + url
			});
			this.ProgressInfo = "g9";
			yield break;
		}
		Global.CurrentMapSettingsLoader = this.www.assetBundle;
		this.ProgressInfo = "g10";
		if (Global.isLoadMap(this.MapCode))
		{
			List<DownLoadData> downLoadInfos = new List<DownLoadData>();
			List<int> monsertidList = Global.GetMonsterListByMapCod(this.MapCode);
			if (monsertidList != null)
			{
				for (int i = 0; i < monsertidList.Count; i++)
				{
					DownLoadData data = new DownLoadData();
					data.type = DownLoadType.Monster;
					data.Key = this.MapCode;
					data.Value = monsertidList[i];
					if (!downLoadInfos.Contains(data))
					{
						downLoadInfos.Add(data);
					}
				}
			}
			List<int> mNPCIdList = Global.GetNPCListByMapCod(this.MapCode);
			if (mNPCIdList != null && mNPCIdList.Count > 0)
			{
				for (int j = 0; j < mNPCIdList.Count; j++)
				{
					DownLoadData data2 = new DownLoadData();
					data2.type = DownLoadType.NPC;
					data2.Key = this.MapCode;
					data2.Value = mNPCIdList[j];
					if (!downLoadInfos.Contains(data2))
					{
						downLoadInfos.Add(data2);
					}
				}
			}
			if (monsertidList != null && monsertidList.Count > 0)
			{
				for (int k = 0; k < monsertidList.Count; k++)
				{
					MonsterVO monsterV = ConfigMonsters.GetMonsterXmlNodeByID(monsertidList[k]);
					if (monsterV == null || !Global.DownLoadInfos.Contains(monsterV.ResName))
					{
						List<int> mDecoIdList = Global.GetMonsterDecoId(monsertidList[k]);
						if (mDecoIdList != null && mDecoIdList.Count > 0)
						{
							for (int l = 0; l < mDecoIdList.Count; l++)
							{
								DownLoadData data3 = new DownLoadData();
								data3.type = DownLoadType.Decoration;
								data3.Key = this.MapCode;
								data3.Value = mDecoIdList[l];
								if (!downLoadInfos.Contains(data3))
								{
									downLoadInfos.Add(data3);
								}
							}
						}
					}
				}
			}
			DownLoadData mapData = new DownLoadData();
			mapData.Key = this.MapCode;
			mapData.Value = this.MapCode;
			mapData.type = DownLoadType.Map;
			if (!downLoadInfos.Contains(mapData))
			{
				downLoadInfos.Add(mapData);
			}
			float fileSize = 0f;
			float ReadySize = 0f;
			for (int m = 0; m < downLoadInfos.Count; m++)
			{
				if (downLoadInfos[m].type == DownLoadType.Map)
				{
					SettingMapVO vo = ConfigSettings.GetSettingMapVOByCode(downLoadInfos[m].Key);
					fileSize += (float)vo.FileSize;
				}
				else if (downLoadInfos[m].type == DownLoadType.Monster)
				{
					fileSize += 800f;
				}
				else if (downLoadInfos[m].type == DownLoadType.NPC)
				{
					fileSize += 800f;
				}
				else if (downLoadInfos[m].type == DownLoadType.Decoration)
				{
					fileSize += 80f;
				}
			}
			if ((float)(QMQJJava.GetLeftAvailableMemery() * 1024L * 1024L) < fileSize)
			{
				string needSpace = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					fileSize + "KB"
				});
				string speceType = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					(!QMQJJava.CheckSDCard()) ? Global.GetLang("内部机身存储") : Global.GetLang("SD卡存储")
				});
				string hintStr = string.Format(Global.GetLang("本次更新至少需{0}的可用{1}空间,您的内存不足"), needSpace, speceType);
				Super.HintMainText(hintStr, 10, 3);
				this.BackToLogin(2, hintStr);
				yield break;
			}
			for (int n = 0; n < downLoadInfos.Count; n++)
			{
				string fileName = string.Empty;
				string filePath = string.Empty;
				string loadMapurl = string.Empty;
				filePath = Global.ReadXmlConfigStr(Global.NetVersionXML, string.Empty, "URL");
				float mCurrentFileSize = 0f;
				if (downLoadInfos[n].type == DownLoadType.Monster)
				{
					MonsterVO monsterV2 = ConfigMonsters.GetMonsterXmlNodeByID(downLoadInfos[n].Value);
					if (monsterV2 != null)
					{
						mCurrentFileSize = 800f;
						fileName = "Monster/" + monsterV2.ResName;
						loadMapurl = string.Format("{0}{1}", filePath, fileName);
					}
				}
				else if (downLoadInfos[n].type == DownLoadType.NPC)
				{
					NPCInfoVO npcVO = ConfigNPCs.GetNPCVOByID(downLoadInfos[n].Value);
					if (npcVO != null)
					{
						mCurrentFileSize = 800f;
						fileName = "NPC/" + npcVO.ResName;
						loadMapurl = string.Format("{0}{1}", filePath, fileName);
					}
				}
				else if (downLoadInfos[n].type == DownLoadType.Decoration)
				{
					DecorationVO DecoVO = ConfigDecoration.GetDecorationVOByCode(downLoadInfos[n].Value);
					if (DecoVO != null)
					{
						mCurrentFileSize = 80f;
						fileName = "Decoration/" + DecoVO.ResName;
						loadMapurl = string.Format("{0}{1}", filePath, fileName);
					}
				}
				else if (downLoadInfos[n].type == DownLoadType.Map)
				{
					SettingMapVO mapVo = ConfigSettings.GetSettingMapVOByCode(downLoadInfos[n].Value);
					if (mapVo != null)
					{
						fileName = "Map/" + mapVo.ResName;
						mCurrentFileSize = (float)mapVo.FileSize;
						loadMapurl = string.Format("{0}{1}", filePath, fileName);
					}
				}
				if (!BackStageDownloadManager.instance.IsNeedDownloadMapAsset(fileName))
				{
					ReadySize += mCurrentFileSize;
				}
				else
				{
					this.ProgressFormatText = Global.GetLang("正在下载地形资源, 请稍候【{0}%】");
					this.ProgressCenter.Percent = (double)(ReadySize / fileSize);
					this.progressText.text = string.Format(this.ProgressFormatText, ReadySize / fileSize);
					bool isDownLoadOK = false;
					string newUrl = loadMapurl + "?v=" + Context.ResSwfVer;
					MUDebug.Log<string>(new string[]
					{
						"InitGameRes新地址 " + newUrl
					});
					using (this.www = new WWW(newUrl))
					{
						yield return this.www;
						if (!string.IsNullOrEmpty(this.www.error))
						{
							MUDebug.LogError<string>(new string[]
							{
								this.www.error
							});
							this.www = null;
							this.BackToLogin(2, Global.GetLang("资源有误或为不存在1！" + loadMapurl));
							yield break;
						}
						if (this.www.bytes.Length > 0 && string.IsNullOrEmpty(this.www.error))
						{
							string path = string.Empty;
							if (downLoadInfos[n].type == DownLoadType.Monster)
							{
								path = PathUtils.GetPersistentPath(fileName);
							}
							else if (downLoadInfos[n].type == DownLoadType.NPC)
							{
								path = PathUtils.GetPersistentPath(fileName);
							}
							else if (downLoadInfos[n].type == DownLoadType.Decoration)
							{
								path = PathUtils.GetPersistentPath(fileName);
							}
							else if (downLoadInfos[n].type == DownLoadType.Map)
							{
								path = PathUtils.GetPersistentPath(fileName);
							}
							isDownLoadOK = Global.SaveBytesToFile(path, this.www.bytes);
							if (isDownLoadOK)
							{
								this.SaveFilesName(fileName, this.www.bytes.Length);
							}
						}
						if (!isDownLoadOK)
						{
							string needSpace2 = Global.GetColorStringForNGUIText(new object[]
							{
								"ff0000",
								fileSize + "KB"
							});
							string speceType2 = Global.GetColorStringForNGUIText(new object[]
							{
								"ff0000",
								(!QMQJJava.CheckSDCard()) ? Global.GetLang("内部机身存储") : Global.GetLang("SD卡存储")
							});
							string hintStr2 = string.Format(Global.GetLang("本次更新至少需{0}的可用{1}空间,您的内存不足"), needSpace2, speceType2);
							Super.HintMainText(hintStr2, 10, 3);
							this.BackToLogin(2, hintStr2);
							yield break;
						}
					}
				}
			}
			this.SaveFilesToLocal();
			if (monsertidList != null)
			{
				monsertidList.Clear();
			}
			if (downLoadInfos != null)
			{
				downLoadInfos.Clear();
			}
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"当前没有过图下载资源111"
			});
		}
		this.ProgressFormatText = Global.GetLang("正在下载地图场景资源, 请稍候【{0}%】");
		this.ProgressCenter.Percent = 0.0;
		this.progressText.Text = string.Format(this.ProgressFormatText, 0);
		url = PathUtils.WebPath(StringUtil.substitute("Map/{0}", new object[]
		{
			ConfigSettings.GetMap3DResNameByCode(this.MapCode)
		}));
		this.ProgressInfo = "g12";
		this.www = new WWW(url);
		yield return this.www;
		this.ProgressInfo = "g13";
		if (!string.IsNullOrEmpty(this.www.error))
		{
			this.WebErrMsg.Text = Global.GetLang("因为网络原因加载地图场景失败..." + this.www.error + "\n" + url);
			this.WebErrMsg.Visibility = true;
			MUDebug.LogError<string>(new string[]
			{
				"加载Map场景失败:" + url
			});
			this.ProgressInfo = "g14";
		}
		else
		{
			Global.CurrentTerrainLoader = this.www.assetBundle;
			this.ProgressInfo = "g15";
		}
		this.ProgressFormatText = Global.GetLang("正在加载地图场景资源, 请稍候【{0}%】");
		this.ProgressCenter.Percent = 0.0;
		this.ProgressShowPercent = 0.01;
		this.progressText.Text = string.Format(this.ProgressFormatText, 0);
		string levelName = this.GetLevelName(url);
		if (Global.Data.ServerData != null && Global.Data.ServerData.LastServer != null && Global.Data.ServerData.LastServer.nFirstLevelServerID != 10 && Global.Data.ServerData.ServerListData == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				Global.GetLang("加载ServerList.xml")
			});
			url = Global.ServerListURL + "GetServerListZt.aspx";
			ZtClientServerListData clientListData = new ZtClientServerListData();
			string strUID = Global.StringReplaceAll(Super.GetXapParamByName("uid", string.Empty), ":", string.Empty);
			clientListData.strUID = strUID;
			clientListData.lTime = TimeManager.GetCorrectLocalTime();
			clientListData.strMD5 = MD5Helper.get_md5_string("HWjKO26fEJvZ27f8v0Qu9EGZ3k3phFO4NCt8A" + clientListData.lTime.ToString());
			byte[] clientBytes = DataHelper.ObjectToBytes<ZtClientServerListData>(clientListData);
			this.www = new WWW(url, clientBytes);
			yield return this.www;
			if (!string.IsNullOrEmpty(this.www.error))
			{
				Super.HideNetWaiting();
				yield break;
			}
			ZtBuffServerListData listData = DataHelper.BytesToObject<ZtBuffServerListData>(this.www.bytes, 0, this.www.bytes.Length);
			url = Global.ServerListCrossPlatfomURL + "GetServerListZt.aspx";
			WWW wwwCrossPlatform = new WWW(url, clientBytes);
			yield return wwwCrossPlatform;
			if (string.IsNullOrEmpty(wwwCrossPlatform.error))
			{
				ZtBuffServerListData listDataExCrossPlatform = DataHelper.BytesToObject<ZtBuffServerListData>(wwwCrossPlatform.bytes, 0, wwwCrossPlatform.bytes.Length);
				if (listDataExCrossPlatform != null)
				{
					listData.listServerData.InsertRange(0, listDataExCrossPlatform.listServerData);
				}
			}
			if (!listData.IsAllPause)
			{
				if (listData.listServerData.Count == 0)
				{
					MUDebug.LogError<string>(new string[]
					{
						Global.GetLang("没有找到远程的ServerList.xml")
					});
					yield break;
				}
				Global.Data.ServerData.ServerListData = listData;
			}
			this.www.Dispose();
			this.www = null;
		}
		this.ProgressInfo = "g16";
		base.StartCoroutine<bool>(this.LoadMapFinish(levelName, url));
		yield break;
	}

	private void StartDownloadMap()
	{
		Super.HideNetWaiting();
		if (Global.IsPopupDownloadMapWindow(this.MapCode) && Global.IsKuaFuHuoDongFenBaoMap && !Global.IsFuBenFenBaoMap)
		{
			this.ProgressInfo = "s1";
			string[] array = new string[]
			{
				Global.GetLang("立即下载"),
				Global.GetLang("稍后下载")
			};
			string message = string.Format(Global.GetLang("当前需要下载场景资源【{0}KB】，是否立即下载？"), Global.GetFenBaoMapSize(this.MapCode));
			NoTitleWindow titleWindow = Super.ShowDialogBox(MainGame._current.Stage, 1, message, (int)Global.GlobalMainWindow.ActualWidth / 2 - 150, 100, 0, string.Empty, array[0], array[1]);
			titleWindow.transform.localPosition = new Vector3(titleWindow.transform.localPosition.x, titleWindow.transform.localPosition.y, -1000f);
			MyDialogBoxPart componentInChildren = titleWindow.GetComponentInChildren<MyDialogBoxPart>();
			if (componentInChildren != null)
			{
				componentInChildren.HintText_Label.transform.localPosition = new Vector3(-148f, 36f, -0.1f);
				componentInChildren.HintText_Label.MaxWidth = 318.0;
			}
			titleWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int dialogBoxReturn = (s1 as NoTitleWindow).DialogBoxReturn;
				Super.CloseNoTitleWindow(MainGame._current.Stage, s1 as NoTitleWindow);
				titleWindow = null;
				if (dialogBoxReturn == 0)
				{
					Global.IsKuaFuHuoDongFenBaoMap = false;
					this.StartCoroutine<bool>(this.InitMapRes());
					this.ProgressInfo = "s2";
				}
				else
				{
					Global.IsKuaFuHuoDongFenBaoMap = false;
					this.BackToLogin(3, string.Empty);
					this.ProgressInfo = "s3";
				}
				return true;
			};
		}
		else
		{
			Global.IsKuaFuHuoDongFenBaoMap = false;
			base.StartCoroutine<bool>(this.InitMapRes());
			Global.IsFuBenFenBaoMap = false;
			this.ProgressInfo = "s4";
		}
	}

	private void BackToLogin(int type, string info = "")
	{
		Object.Destroy(base.gameObject);
		if (PlayZone.GlobalPlayZone != null)
		{
			PlayZone.GlobalPlayZone.ClearDataWhenDownloadFail();
		}
		else
		{
			U3DUtils.ClearAll3DObjects(true, true);
			Super.ConnectToGameServerFailed = true;
			GameInstance.Game.Disconnect();
			GameInstance.CreateNewTCPGame();
			if (Super.MainGameMgr != null)
			{
				Object.Destroy(Super.MainGameMgr.gameObject);
			}
			if (null != Global.CurrentMapSettingsLoader)
			{
				Global.CurrentMapSettingsLoader.Unload(true);
				Global.CurrentMapSettingsLoader = null;
			}
			KuaFuLoginManager.ClearLoginInfo();
			Global.Data.CurrentTeamData = null;
			string loginMode = Global.GetLoginMode();
			if ("0" != loginMode)
			{
				PlatformUserLogin platformUserLogin = Super.ShowPlatformUserLogin(Super.MainWindowRoot, true);
			}
			else
			{
				Super.ShowUserLogin(Super.MainWindowRoot);
			}
		}
		if (type == 1)
		{
			info = Global.GetLang("勇士,您的网络已断开连接,请检查您的网络是否正常！");
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), info, -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				return true;
			};
		}
		else if (type == 2)
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), info, -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				return true;
			};
		}
	}

	public static void ClearSpeicalMapEffect()
	{
		int childCount = Global.MainCamera.transform.childCount;
		for (int i = childCount - 1; i >= 0; i--)
		{
			Object.Destroy(Global.MainCamera.transform.GetChild(i).gameObject);
		}
	}

	private IEnumerator InitMapRes()
	{
		BackStageDownloadManager.instance.StopDownloadByExternal();
		this.StartUITimer();
		this.ProgressInfo = "m1";
		if (null != Global.AudioListener43D)
		{
			Global.AudioListener43D.enabled = true;
		}
		Global.BackgroundAudio43D.StopPlay();
		if (null != Global.BackgroundAudio43D && !Global.Data.SysSetting.CloseGameMusic)
		{
			Global.BackgroundAudio43D.PlayAudio(ConfigSettings.GetMapMusicFileByCode(this.MapCode, true), true, false);
		}
		MuAssetManager.Instance.UnLoadAllUnuseResource();
		if (!ZoneLoader.DisableSliceTerrain)
		{
			ZoneLoader.ReleaseSingleton();
		}
		this.StopCameraShake();
		this.ProgressFormatText = Global.GetLang("正在加载地形配置文件, 请稍候【{0}%】");
		this.ProgressCenter.Percent = 0.0;
		this.progressText.Text = string.Format(this.ProgressFormatText, 0);
		this.ProgressInfo = "m2";
		string url = Global.WebPath(StringUtil.substitute("MapConfig/MapConfig{0}.unity3d", new object[]
		{
			ConfigSettings.GetMapPicCodeByCode(this.MapCode)
		}));
		this.www = new WWW(url);
		yield return this.www;
		this.ProgressInfo = "m3";
		if (!string.IsNullOrEmpty(this.www.error))
		{
			this.WebErrMsg.Text = Global.GetLang("因为网络原因加载地形配置文件失败...");
			this.WebErrMsg.Visibility = true;
			MUDebug.LogError<string>(new string[]
			{
				"加载Map地图信息失败:" + url
			});
			this.OnFatalError();
			this.ProgressInfo = "m4";
			yield break;
		}
		Global.CurrentMapLoader = this.www.assetBundle;
		this.ProgressInfo = "m5";
		if (null == Global.CurrentMapSettingsLoader)
		{
			this.ProgressFormatText = Global.GetLang("正在加载地图配置文件, 请稍候【{0}%】");
			this.ProgressCenter.Percent = 0.0;
			this.progressText.Text = string.Format(this.ProgressFormatText, 0);
			url = Global.WebPath(StringUtil.substitute("GameSite/{0}/Map/Map{1}.unity3d", new object[]
			{
				Global.Data.GamePingTaiID,
				ConfigSettings.GetMapPicCodeByCode(this.MapCode)
			}));
			this.www = new WWW(url);
			this.ProgressInfo = "m6";
			yield return this.www;
			if (!string.IsNullOrEmpty(this.www.error))
			{
				this.WebErrMsg.Text = Global.GetLang("因为网络原因加载地图配置文件失败...");
				this.WebErrMsg.Visibility = true;
				MUDebug.LogError<string>(new string[]
				{
					"加载Map配置失败:" + url
				});
				this.ProgressInfo = "m7";
				yield break;
			}
			Global.CurrentMapSettingsLoader = this.www.assetBundle;
			this.ProgressInfo = "m8";
		}
		if (Global.isLoadMap(this.MapCode))
		{
			this.ProgressInfo = "m10";
			List<DownLoadData> downLoadInfos = new List<DownLoadData>();
			List<int> monsertidList = Global.GetMonsterListByMapCod(this.MapCode);
			if (monsertidList != null)
			{
				for (int i = 0; i < monsertidList.Count; i++)
				{
					DownLoadData data = new DownLoadData();
					data.type = DownLoadType.Monster;
					data.Key = this.MapCode;
					data.Value = monsertidList[i];
					if (!downLoadInfos.Contains(data))
					{
						downLoadInfos.Add(data);
					}
				}
			}
			this.ProgressInfo = "m11";
			List<int> mNPCIdList = Global.GetNPCListByMapCod(this.MapCode);
			if (mNPCIdList != null && mNPCIdList.Count > 0)
			{
				for (int j = 0; j < mNPCIdList.Count; j++)
				{
					DownLoadData data2 = new DownLoadData();
					data2.type = DownLoadType.NPC;
					data2.Key = this.MapCode;
					data2.Value = mNPCIdList[j];
					if (!downLoadInfos.Contains(data2))
					{
						downLoadInfos.Add(data2);
					}
				}
			}
			this.ProgressInfo = "m12";
			if (monsertidList != null && monsertidList.Count > 0)
			{
				for (int k = 0; k < monsertidList.Count; k++)
				{
					MonsterVO monsterV = ConfigMonsters.GetMonsterXmlNodeByID(monsertidList[k]);
					if (monsterV == null || !Global.DownLoadInfos.Contains(monsterV.ResName))
					{
						List<int> mDecoIdList = Global.GetMonsterDecoId(monsertidList[k]);
						if (mDecoIdList != null && mDecoIdList.Count > 0)
						{
							for (int l = 0; l < mDecoIdList.Count; l++)
							{
								DownLoadData data3 = new DownLoadData();
								data3.type = DownLoadType.Decoration;
								data3.Key = this.MapCode;
								data3.Value = mDecoIdList[l];
								if (!downLoadInfos.Contains(data3))
								{
									downLoadInfos.Add(data3);
								}
							}
						}
					}
				}
			}
			this.ProgressInfo = "m13";
			DownLoadData mapData = new DownLoadData();
			mapData.Key = this.MapCode;
			mapData.Value = this.MapCode;
			mapData.type = DownLoadType.Map;
			if (!downLoadInfos.Contains(mapData))
			{
				downLoadInfos.Add(mapData);
			}
			float fileSize = 0f;
			float ReadySize = 0f;
			for (int m = 0; m < downLoadInfos.Count; m++)
			{
				if (downLoadInfos[m].type == DownLoadType.Map)
				{
					SettingMapVO vo = ConfigSettings.GetSettingMapVOByCode(downLoadInfos[m].Key);
					fileSize += (float)vo.FileSize;
				}
				else if (downLoadInfos[m].type == DownLoadType.Monster)
				{
					fileSize += 800f;
				}
				else if (downLoadInfos[m].type == DownLoadType.NPC)
				{
					fileSize += 800f;
				}
				else if (downLoadInfos[m].type == DownLoadType.Decoration)
				{
					fileSize += 80f;
				}
			}
			this.ProgressInfo = "m14";
			if ((float)(QMQJJava.GetLeftAvailableMemery() * 1024L * 1024L) < fileSize)
			{
				string needSpace = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					fileSize + "KB"
				});
				string speceType = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					(!QMQJJava.CheckSDCard()) ? Global.GetLang("内部机身存储") : Global.GetLang("SD卡存储")
				});
				string hintStr = string.Format(Global.GetLang("本次更新至少需{0}的可用{1}空间,您的内存不足"), needSpace, speceType);
				Super.HintMainText(hintStr, 10, 3);
				this.ProgressInfo = "m15";
				this.BackToLogin(2, hintStr);
				yield break;
			}
			for (int n = 0; n < downLoadInfos.Count; n++)
			{
				int donwLoadCode = downLoadInfos[n].Value;
				string fileName = string.Empty;
				string filePath = string.Empty;
				string loadMapurl = string.Empty;
				filePath = Global.ReadXmlConfigStr(Global.NetVersionXML, string.Empty, "URL");
				float mCurrentFileSize = 0f;
				if (downLoadInfos[n].type == DownLoadType.Monster)
				{
					MonsterVO monsterV2 = ConfigMonsters.GetMonsterXmlNodeByID(downLoadInfos[n].Value);
					if (monsterV2 != null)
					{
						mCurrentFileSize = 800f;
						fileName = "Monster/" + monsterV2.ResName;
						loadMapurl = string.Format("{0}{1}", filePath, fileName);
					}
				}
				else if (downLoadInfos[n].type == DownLoadType.NPC)
				{
					NPCInfoVO npcVO = ConfigNPCs.GetNPCVOByID(downLoadInfos[n].Value);
					if (npcVO != null)
					{
						mCurrentFileSize = 800f;
						fileName = "NPC/" + npcVO.ResName;
						loadMapurl = string.Format("{0}{1}", filePath, fileName);
					}
				}
				else if (downLoadInfos[n].type == DownLoadType.Decoration)
				{
					DecorationVO DecoVO = ConfigDecoration.GetDecorationVOByCode(downLoadInfos[n].Value);
					if (DecoVO != null)
					{
						mCurrentFileSize = 80f;
						fileName = "Decoration/" + DecoVO.ResName;
						loadMapurl = string.Format("{0}{1}", filePath, fileName);
					}
				}
				else if (downLoadInfos[n].type == DownLoadType.Map)
				{
					SettingMapVO mapVo = ConfigSettings.GetSettingMapVOByCode(downLoadInfos[n].Value);
					if (mapVo != null)
					{
						mCurrentFileSize = (float)mapVo.FileSize;
						fileName = "Map/" + mapVo.ResName;
						loadMapurl = string.Format("{0}{1}", filePath, fileName);
					}
				}
				if (!BackStageDownloadManager.instance.IsNeedDownloadMapAsset(fileName))
				{
					ReadySize += mCurrentFileSize;
				}
				else
				{
					this.ProgressFormatText = Global.GetLang("正在下载地形资源, 请稍候【{0}%】");
					this.ProgressCenter.Percent = (double)(ReadySize / fileSize);
					this.progressText.text = string.Format(this.ProgressFormatText, ReadySize / fileSize);
					bool isDownLoadOK = false;
					string newUrl = loadMapurl + "?v=" + Context.ResSwfVer;
					MUDebug.Log<string>(new string[]
					{
						"InitMapRes新地址 " + newUrl
					});
					using (this.www = new WWW(newUrl))
					{
						yield return this.www;
						if (!string.IsNullOrEmpty(this.www.error))
						{
							MUDebug.LogError<string>(new string[]
							{
								this.www.error
							});
							this.www = null;
							this.BackToLogin(2, Global.GetLang("资源下载有误或不存在2！" + loadMapurl));
							yield break;
						}
						if (this.www.bytes.Length > 0 && string.IsNullOrEmpty(this.www.error))
						{
							string path = string.Empty;
							if (downLoadInfos[n].type == DownLoadType.Monster)
							{
								path = PathUtils.GetPersistentPath(fileName);
							}
							else if (downLoadInfos[n].type == DownLoadType.NPC)
							{
								path = PathUtils.GetPersistentPath(fileName);
							}
							else if (downLoadInfos[n].type == DownLoadType.Decoration)
							{
								path = PathUtils.GetPersistentPath(fileName);
							}
							else if (downLoadInfos[n].type == DownLoadType.Map)
							{
								path = PathUtils.GetPersistentPath(fileName);
							}
							isDownLoadOK = Global.SaveBytesToFile(path, this.www.bytes);
							if (isDownLoadOK)
							{
								this.SaveFilesName(fileName, this.www.bytes.Length);
							}
						}
						if (!isDownLoadOK)
						{
							string needSpace2 = Global.GetColorStringForNGUIText(new object[]
							{
								"ff0000",
								fileSize + "KB"
							});
							string speceType2 = Global.GetColorStringForNGUIText(new object[]
							{
								"ff0000",
								(!QMQJJava.CheckSDCard()) ? Global.GetLang("内部机身存储") : Global.GetLang("SD卡存储")
							});
							string hintStr2 = string.Format(Global.GetLang("本次更新至少需{0}的可用{1}空间,您的内存不足"), needSpace2, speceType2);
							Super.HintMainText(hintStr2, 10, 3);
							this.BackToLogin(2, hintStr2);
							this.ProgressInfo = "m16";
							yield break;
						}
					}
				}
			}
			this.SaveFilesToLocal();
			if (monsertidList != null)
			{
				monsertidList.Clear();
			}
			if (downLoadInfos != null)
			{
				downLoadInfos.Clear();
			}
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"当前没有过图下载资源222"
			});
		}
		this.ProgressInfo = "m17";
		this.ProgressFormatText = Global.GetLang("正在下载地图场景资源, 请稍候【{0}%】");
		this.ProgressCenter.Percent = 0.0;
		this.progressText.Text = string.Format(this.ProgressFormatText, 0);
		url = PathUtils.WebPath(StringUtil.substitute("Map/{0}", new object[]
		{
			ConfigSettings.GetMap3DResNameByCode(this.MapCode)
		}));
		this.www = new WWW(url);
		yield return this.www;
		this.ProgressInfo = "m18";
		if (!string.IsNullOrEmpty(this.www.error))
		{
			this.WebErrMsg.Text = Global.GetLang("因为网络原因加载地图场景失败..." + this.www.error + "\n" + url);
			this.WebErrMsg.Visibility = true;
			this.ProgressInfo = "m19";
		}
		else
		{
			Global.CurrentTerrainLoader = this.www.assetBundle;
			this.ProgressInfo = "m20";
		}
		this.ProgressFormatText = Global.GetLang("正在加载地图场景资源, 请稍候【{0}%】");
		this.ProgressCenter.Percent = 0.0;
		this.ProgressShowPercent = 0.01;
		this.progressText.Text = string.Format(this.ProgressFormatText, 0);
		string levelName = this.GetLevelName(url);
		LoadingMap.ClearConfigData();
		base.StartCoroutine<bool>(this.LoadMapFinish(levelName, url));
		this.ProgressInfo = "m21";
		yield break;
	}

	public void OnFatalError()
	{
		Super.HintMainText(Global.GetLang("致命错误:目标地图无法加载,自动传送到勇者大陆!"), 10, 3);
		this.MapCode = 1;
		this.LoadType = this._LoadType;
		this.FatalErrorHandler = delegate(object s, EventArgs e)
		{
			GameInstance.Game.SpriteGotToMap(1);
		};
	}

	private string GetLevelName(string url)
	{
		int num = url.LastIndexOf('/');
		if (num < 0)
		{
			return string.Empty;
		}
		int num2 = url.LastIndexOf('.');
		if (num2 < 0)
		{
			return string.Empty;
		}
		return url.Substring(num + 1, num2 - num - 1);
	}

	private void StartUITimer()
	{
		if (this.UITimer != null)
		{
			this.StopUITimer();
		}
		this.UITimer = new DispatcherTimer("LoadingMap_UITimer");
		this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		this.UITimer.Start();
		this.HeartTimer = new DispatcherTimer("LoadingMap_HeartTimer");
		this.HeartTimer.Interval = TimeSpan.FromSeconds(10.0);
		this.HeartTimer.Tick = new DispatcherTimerEventHandler(this.HeartTimer_Tick);
		this.HeartTimer.Start();
		this.progressText.Text = StringUtil.substitute(Global.GetLang("开始初始化游戏, 请稍候"), new object[0]);
	}

	private void StopUITimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Stop();
			this.UITimer.Tick = null;
			this.UITimer = null;
		}
		if (this.HeartTimer != null)
		{
			this.HeartTimer.Stop();
			this.HeartTimer.Tick = null;
			this.HeartTimer = null;
		}
	}

	private void HeartTimer_Tick(object sender, object e)
	{
		BasePlayZone.InWaitPingCount = 0;
		GameInstance.Game.SpriteHeart();
	}

	private void UITimer_Tick(object sender, object e)
	{
		if (this.www != null)
		{
			if (this.www.isDone)
			{
				this.ProgressPercent = 1.0;
				this.progressText.Text = string.Format(this.ProgressFormatText, 100);
			}
			else
			{
				this.ProgressPercent = (double)this.www.progress;
				this.progressText.Text = string.Format(this.ProgressFormatText, (int)((double)this.www.progress * 100.0));
			}
		}
		if (this._AsyncOperation != null)
		{
			this.ProgressInfo = "t1";
			if (this._AsyncOperation.isDone)
			{
				this.ProgressPercent = 1.0;
				this.progressText.Text = StringUtil.substitute(this.ProgressFormatText, new object[]
				{
					100
				});
				this.ProgressInfo = "t2";
				this.StopUITimer();
				if (ConfigSystemParam.GetSystemParamIntByName("IsCheckPlatformLogin") != 1L && Super.platformLogin != null)
				{
					MUDebug.LogError<string>(new string[]
					{
						"LoadingMap Finish: Super.platformLogin != null"
					});
					return;
				}
				RenderSettings.ambientLight = new Color(0.8f, 0.8f, 0.8f);
				PerformanceCtrl.Fog = RenderSettings.fog;
				this.CopyCameraParmas();
				this.ProgressInfo = "t3";
				if (this.WorkFinished != null)
				{
					this.WorkFinished.Invoke(this, EventArgs.Empty);
					this.ProgressInfo = "t4";
				}
				if (this.FatalErrorHandler != null)
				{
					this.FatalErrorHandler.Invoke(null, null);
					this.FatalErrorHandler = null;
					this.ProgressInfo = "t5";
				}
				this.ProgressInfo = "t6";
				this.AddMapEffectManager();
				MonsterCachingManager.ClearCachingItems();
				if (Global.Data != null && Global.Data.GameRadarMap != null)
				{
					Global.Data.GameRadarMap.ClearAllRolePoints();
				}
				Resources.UnloadUnusedAssets();
				this.ProgressInfo = "t7";
				BackStageDownloadManager.instance.StartDownloadByExternal();
				this.ProgressInfo = "t8";
			}
			else
			{
				if (this.ProgressShowPercent < (double)this._AsyncOperation.progress && (double)this._AsyncOperation.progress > 0.95)
				{
					this.ProgressPercent = (double)this._AsyncOperation.progress;
				}
				else
				{
					if (this.ProgressShowPercent < 0.6)
					{
						this.ProgressShowPercent += 0.01;
					}
					else if (this.ProgressShowPercent < 0.8)
					{
						this.ProgressShowPercent += 0.005;
					}
					else if (this.ProgressShowPercent < 0.9)
					{
						this.ProgressShowPercent += 0.001;
					}
					else if (this.ProgressShowPercent < 0.95)
					{
						this.ProgressShowPercent += 0.0005;
					}
					this.ProgressPercent = this.ProgressShowPercent;
				}
				this.progressText.Text = StringUtil.substitute(this.ProgressFormatText, new object[]
				{
					(int)(this.ProgressPercent * 100.0)
				});
			}
		}
	}

	private IEnumerator LoadMapFinish(string levelName, string url)
	{
		this.ProgressInfo = "f1";
		this._AsyncOperation = Application.LoadLevelAsync(levelName);
		this.ProgressInfo = "f2";
		if (this._AsyncOperation == null)
		{
			this.ProgressInfo = "f3";
			try
			{
				this.ProgressFormatText = Global.GetLang("正在加载地图场景资源, 请稍候【{0}%】");
				this.ProgressCenter.Percent = 0.0;
				this.ProgressShowPercent = 0.01;
				this.progressText.Text = string.Format(this.ProgressFormatText, 0);
				AssetBundle ab = Global.CurrentTerrainLoader;
				if (ab != null)
				{
					ab.Unload(true);
				}
				Global.CurrentTerrainLoader = null;
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				MUDebug.LogException(ex);
			}
			this.ProgressInfo = "f4";
			AsyncOperation ao = Resources.UnloadUnusedAssets();
			yield return ao;
			this.ProgressInfo = "f5";
			bool isReDownloadResOk = false;
			if (ConfigSystemParam.GetSystemParamIntByName("IsDoReDownLoadMap") == 1L)
			{
				MUDebug.LogError<string>(new string[]
				{
					"---DoReDownLoadMap 重新下载地图 ---"
				});
				string filePath = null;
				filePath = Global.ReadXmlConfigStr(Global.NetVersionXML, string.Empty, "URL");
				string newUrl = string.Concat(new object[]
				{
					filePath,
					"Map/",
					levelName,
					".unity3d?v=",
					Time.realtimeSinceStartup
				});
				MUDebug.Log<string>(new string[]
				{
					"出错重新下载 " + newUrl
				});
				this.www = new WWW(newUrl);
				yield return this.www;
				if (!string.IsNullOrEmpty(this.www.error))
				{
					this.WebErrMsg.Text = Global.GetLang("因为网络原因加载地图场景失败... ..." + this.www.error + "\n" + filePath);
					this.WebErrMsg.Visibility = true;
					yield break;
				}
				string resName = "Map/" + levelName + ".unity3d";
				string path = PathUtils.GetPersistentPath(resName);
				isReDownloadResOk = Global.SaveBytesToFile(path, this.www.bytes);
				if (isReDownloadResOk)
				{
					url = PathUtils.WebPath(resName);
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						"资源下载保存错误！！！" + path
					});
				}
			}
			this.www = new WWW(url);
			yield return this.www;
			this.ProgressInfo = "f6";
			if (!string.IsNullOrEmpty(this.www.error))
			{
				this.WebErrMsg.Text = Global.GetLang("因为网络原因加载地图场景失败..." + this.www.error + "\n" + url);
				this.WebErrMsg.Visibility = true;
				MUDebug.Log<string>(new string[]
				{
					"f7 下载出错地址：" + url
				});
				this.ProgressInfo = "f7";
				yield break;
			}
			this.ProgressInfo = "f8";
			Global.CurrentTerrainLoader = this.www.assetBundle;
			base.StartCoroutine<bool>(this.LoadMapFinish(levelName, url));
		}
		else
		{
			yield return this._AsyncOperation;
			this.ProgressInfo = "f9";
			Global.IsInGameScene = true;
			if (this.www != null)
			{
				this.www.Dispose();
				this.www = null;
			}
		}
		this.ProgressInfo = "f10";
		yield break;
	}

	public static void ClearConfigData()
	{
		ConfigManager.ChangeScene();
		ConfigYuanSuJueXing.instance.ClearDataChangeScene();
		Global.ClearXmlDataOnChangeScene();
		AlchemyPart.ClearXMLData();
		AoYunDaTiPart.ClearXMLData();
		CompeteCityPart.ClearXMLData();
		LoversWishPart.ClearXMLDataOnChageScene();
		ShenJiJiFenZhuRu.ClearXMLData();
		ShenShiPart.ClearXMLData();
		ShenShiPartFuWenXiangQian.ClearXMLData();
		ShenShiPartTuiJian.ClearXMLData();
		SoulCometStoneGathering.ClearXMLData();
		TerritoryFightPart.ClearXMLData();
		PetBossXMLConfigManager.Clear();
		YaoSaiJianYuPartJiLu.ClearXMLData();
		ZhanMengShenDianPart.ClearXMLData();
		ZhongShenZhengBaPart.ClearXMLData();
		ZuduiFuBen_LangHunYaoSai.ClearXMLData();
		OpenServerActiveLevelPart.ClearXMLData();
		TuJianPart.ClearXMLData();
		TeamCompeteDataManager.CloseAllTipWindow();
		DaTaoShaDataManager.Clear();
	}

	public static void ReShader()
	{
		GameObject gameObject = GameObject.Find("Scene_xuanjue");
		if (gameObject)
		{
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					if (sharedMaterials[j])
					{
						int renderQueue = sharedMaterials[j].renderQueue;
						sharedMaterials[j].shader = Shader.Find(sharedMaterials[j].shader.name);
						sharedMaterials[j].renderQueue = renderQueue;
					}
				}
			}
		}
		GameObject gameObject2 = GameObject.Find("scene");
		if (!gameObject2)
		{
			gameObject2 = GameObject.Find("Scene");
		}
		if (!gameObject2)
		{
			return;
		}
		Renderer[] componentsInChildren2 = gameObject2.GetComponentsInChildren<Renderer>(true);
		for (int k = 0; k < componentsInChildren2.Length; k++)
		{
			Material[] sharedMaterials2 = componentsInChildren2[k].sharedMaterials;
			for (int l = 0; l < sharedMaterials2.Length; l++)
			{
				if (sharedMaterials2[l])
				{
					sharedMaterials2[l].shader = Shader.Find(sharedMaterials2[l].shader.name);
				}
			}
		}
	}

	private IEnumerator LoadEmptyScene()
	{
		this.ProgressPercent = 0.0;
		float depth = Global.UICamera.depth;
		Global.UICamera.depth = 0f;
		yield return 2;
		this.ProgressInfo = "e1";
		LoadingMap.ClearSpeicalMapEffect();
		this.ClearMapEffectManager();
		GTipServiceEx.CloseBaoXiangTipWindow();
		ObjectsManager.Clear();
		Application.LoadLevel("empty");
		this.ProgressInfo = "e2";
		XmlManager.ClearPartXml();
		AsyncOperation async = Resources.UnloadUnusedAssets();
		GC.Collect();
		yield return async;
		this.ProgressInfo = "e3";
		Global.UICamera.depth = depth;
		if (this.LoadType == 0)
		{
			this.StartDownloadMap();
			this.ProgressInfo = "e4";
		}
		else if (this.LoadType != 1)
		{
			if (this.LoadType == 2)
			{
				this.StartDownloadGameRes();
				this.ProgressInfo = "e5";
			}
		}
		yield break;
	}

	private void StopCameraShake()
	{
		CameraShake component = Global.MainCamera.GetComponent<CameraShake>();
		if (null != component)
		{
			component.enabled = false;
		}
	}

	private void AddMapEffectManager()
	{
		string text = "Scene/MapEffect";
		GameObject gameObject = U3DUtils.FindGameObjectByName(null, "/" + text);
		if (null != gameObject)
		{
			gameObject.AddComponent<MapEffectManager>();
		}
	}

	private void ClearMapEffectManager()
	{
		string text = "Scene/MapEffect";
		GameObject gameObject = U3DUtils.FindGameObjectByName(null, "/" + text);
		if (null != gameObject && gameObject.GetComponent<MapEffectManager>() != null)
		{
			gameObject.GetComponent<MapEffectManager>().ClearMapEffect();
		}
	}

	private void CopyCameraParmas()
	{
		Global.MainCamera.backgroundColor = Color.black;
		Global.MainCamera.farClipPlane = 50f;
		LayerCullDistanceslMgr.SetCameraLayerDistance(Global.MainCamera, 50f);
		if (Global.GetMapType(this.MapCode) != MapTypes.Normal)
		{
			Global.MainCamera.farClipPlane = 100f;
			LayerCullDistanceslMgr.SetCameraLayerDistance(Global.MainCamera, 100f);
		}
		PerformanceCtrl.PerformanceType = PerformanceCtrl.PerformanceType;
		GameObject gameObject = GameObject.Find("CameraParams");
		if (gameObject != null)
		{
			Camera component = gameObject.GetComponent<Camera>();
			float num = 50f;
			string loadedLevelName = Application.loadedLevelName;
			if (loadedLevelName != null)
			{
				if (LoadingMap.<>f__switch$map5 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(11);
					dictionary.Add("zhenyingzhan", 0);
					dictionary.Add("siwangshamo", 0);
					dictionary.Add("emoguangchang", 0);
					dictionary.Add("xuesechengbao", 0);
					dictionary.Add("xianzongmijing", 0);
					dictionary.Add("zhiyanku", 0);
					dictionary.Add("kulouwangdian", 0);
					dictionary.Add("gedengwangdian", 0);
					dictionary.Add("baluokewangshi", 0);
					dictionary.Add("wanhundian1", 0);
					dictionary.Add("tiankongzhicheng", 0);
					LoadingMap.<>f__switch$map5 = dictionary;
				}
				int num2;
				if (LoadingMap.<>f__switch$map5.TryGetValue(loadedLevelName, ref num2))
				{
					if (num2 == 0)
					{
						num = component.farClipPlane;
					}
				}
			}
			Global.MainCamera.farClipPlane = num;
			LayerCullDistanceslMgr.SetCameraLayerDistance(Global.MainCamera, num);
		}
	}

	public void SaveFilesName(string resName, int bytesLength)
	{
		BackStageDownloadManager.instance.SaveAssetToHaveDownLoadList(resName, bytesLength);
	}

	public void SaveFilesToLocal()
	{
		BackStageDownloadManager.instance.SaveHaveDownLoadXML();
	}

	public GImgProgressBar ProgressCenter;

	public GTextBlockOutLine progressText;

	public GTextBlockOutLine progressText2;

	public GTextBlockOutLine WebErrMsg;

	public TextBlock HintUser1;

	public ShowNetImage MapPic;

	public ShowNetImage LogoPic;

	public UISprite Bak;

	private int _MapCode;

	private int _LoadType;

	public EventHandler WorkFinished;

	public EventHandler CachingNotify;

	private double LoadingMapShowPercent;

	public WWW www;

	private string ProgressFormatText = "{0}";

	private AsyncOperation _AsyncOperation;

	public EventHandler FatalErrorHandler;

	private DispatcherTimer UITimer;

	private DispatcherTimer HeartTimer;
}
