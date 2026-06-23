using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class BackStageDownloadManager : MonoBehaviour
{
	private bool mIsAllResDownload
	{
		get
		{
			return this.mtIsAllResDownload;
		}
		set
		{
			this.mtIsAllResDownload = value;
			if (this.mtIsAllResDownload)
			{
				this.StopWIFITimer();
			}
		}
	}

	private void Awake()
	{
		BackStageDownloadManager.instance = this;
	}

	private bool IsEditorClose()
	{
		return false;
	}

	public IEnumerator InitDownLoadInfo()
	{
		if (this.IsEditorClose())
		{
			yield break;
		}
		if (Global.FenBaoType == FenBaoDownloadType.None)
		{
			MUDebug.LogError<string>(new string[]
			{
				"！！！Version中分包字段为开启！！！"
			});
			yield break;
		}
		if (this.IsAllResDownload)
		{
			this.mIsAllResDownload = true;
			MUDebug.Log<string>(new string[]
			{
				"*******所有资源已经下载完成！*******如果下载地图失败，需要点击Clear清理资源按钮，清除本地缓存"
			});
			yield break;
		}
		MUDebug.Log<string>(new string[]
		{
			"初始化缓存 NotInPackage.xml"
		});
		List<string> AssetsPathList = new List<string>();
		Dictionary<string, BackStageDownloadManager.CfgData> AssetsDict = new Dictionary<string, BackStageDownloadManager.CfgData>();
		string NotInPackagePath = string.Empty;
		NotInPackagePath = Global.ReadXmlConfigStr(Global.NetVersionXML, string.Empty, "URL");
		string downloadXMLName = string.Empty;
		if (Global.FenBaoType == FenBaoDownloadType.JingMo)
		{
			downloadXMLName = "NotInPackage.xml";
		}
		else if (Global.FenBaoType == FenBaoDownloadType.TuiGuang)
		{
			downloadXMLName = "NotInPackage_TuiGuang.xml";
		}
		else if (Global.FenBaoType == FenBaoDownloadType.Map)
		{
			downloadXMLName = "NotInPackage_Map.xml";
		}
		else if (Global.FenBaoType == FenBaoDownloadType.None)
		{
			yield break;
		}
		WWW www = new WWW(string.Concat(new object[]
		{
			NotInPackagePath,
			downloadXMLName,
			"?v=",
			Global.GetTimeStamp()
		}));
		yield return www;
		try
		{
			if (string.IsNullOrEmpty(www.error))
			{
				if (www.text != null)
				{
					XElement xe = XElement.Parse(www.text);
					if (xe != null)
					{
						List<XElement> openL = Global.GetXElementList(xe, "Recorder");
						if (openL != null && openL.Count > 0)
						{
							MUDebug.Log<string>(new string[]
							{
								"Recorder" + Global.GetXElementAttributeInt(openL[0], "Value")
							});
							this.mIsOpen = (Global.GetXElementAttributeInt(openL[0], "Value") == 1);
						}
						else
						{
							this.mIsOpen = false;
						}
						if (this.mIsOpen)
						{
							List<XElement> xelist = Global.GetXElementList(xe, "File");
							if (xelist != null)
							{
								List<BackStageDownloadManager.CfgData> cfgDataList = new List<BackStageDownloadManager.CfgData>();
								for (int i = 0; i < xelist.Count; i++)
								{
									cfgDataList.Add(new BackStageDownloadManager.CfgData
									{
										name = Global.GetXElementAttributeStr(xelist[i], "Name"),
										size = Global.GetXElementAttributeLong(xelist[i], "Size"),
										type = Global.GetXElementAttributeInt(xelist[i], "Type")
									});
								}
								if (cfgDataList.Count > 0)
								{
									cfgDataList.Sort((BackStageDownloadManager.CfgData x, BackStageDownloadManager.CfgData y) => x.type.CompareTo(y.type));
									for (int j = 0; j < cfgDataList.Count; j++)
									{
										BackStageDownloadManager.CfgData data = cfgDataList[j];
										if (!AssetsDict.ContainsKey(data.name))
										{
											AssetsDict.Add(data.name, data);
											this.mNeedDLSize += data.size;
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"downloadXMLName " + www.error
				});
			}
			www = null;
		}
		catch (Exception ex3)
		{
			Exception ex = ex3;
			MUDebug.LogException(ex);
		}
		if (!this.mIsOpen)
		{
			MUDebug.LogError<string>(new string[]
			{
				"未开启   NotInPackage.xml中的Recorder" + this.mIsOpen
			});
			yield break;
		}
		string filePath = PathUtils.GetPersistentPath("NotInPackageAssestDwonLoadMsg.xml");
		if (File.Exists(filePath))
		{
			using (StreamReader sr = new StreamReader(filePath))
			{
				try
				{
					string xmlStr = sr.ReadToEnd();
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(xmlStr);
					XElement xel = doc.DocumentElement;
					if (doc.DocumentElement != null)
					{
						List<XElement> xellist = Global.GetXElementList(xel, "File");
						if (xellist != null)
						{
							string fileName = string.Empty;
							long size = 0L;
							for (int k = 0; k < xellist.Count; k++)
							{
								fileName = Global.GetXElementAttributeStr(xellist[k], "Name");
								size = Global.GetXElementAttributeLong(xellist[k], "Size");
								this.mHaveDownLoadList.Add(new FileData(fileName, size, -1));
							}
						}
					}
				}
				catch (Exception ex4)
				{
					Exception ex2 = ex4;
					MUDebug.LogException(ex2);
				}
				sr.Close();
			}
		}
		this.CDNPath = Global.ReadXmlConfigStr(Global.NetVersionXML, string.Empty, "URL");
		if (AssetsDict != null && AssetsDict.Count > 0)
		{
			Dictionary<string, BackStageDownloadManager.CfgData>.Enumerator itr = AssetsDict.GetEnumerator();
			while (itr.MoveNext())
			{
				if (!this.mHaveDownLoadList.Exists(delegate(FileData x)
				{
					string fileName2 = x.FileName;
					KeyValuePair<string, BackStageDownloadManager.CfgData> keyValuePair2 = itr.Current;
					bool result;
					if (fileName2 == keyValuePair2.Key)
					{
						long fileSize = x.FileSize;
						KeyValuePair<string, BackStageDownloadManager.CfgData> keyValuePair3 = itr.Current;
						result = (fileSize == keyValuePair3.Value.size);
					}
					else
					{
						result = false;
					}
					return result;
				}))
				{
					List<CDNDownLoadData> list = this.mAssetsLoadList;
					KeyValuePair<string, BackStageDownloadManager.CfgData> keyValuePair = itr.Current;
					list.Add(new CDNDownLoadData(keyValuePair.Key, null));
				}
			}
		}
		AssetsDict.Clear();
		this.mDownLoadOk = true;
		if (this.mCurrentNetState == 2)
		{
			this.mChangeNet = true;
		}
		yield break;
	}

	public void AddImgsWaitForCDNAsset(string bundleId, WaitForCDNAsset img)
	{
		List<WaitForCDNAsset> list = null;
		if (this.dictImgsWaitForCDNAsset.TryGetValue(bundleId, ref list))
		{
			List<WaitForCDNAsset> list2 = this.dictImgsWaitForCDNAsset[bundleId];
			list2.Add(img);
		}
		else
		{
			List<WaitForCDNAsset> list3 = new List<WaitForCDNAsset>();
			list3.Add(img);
			this.dictImgsWaitForCDNAsset.Add(bundleId, list3);
		}
	}

	public void ReleaseImgs(string bundleIdPath)
	{
		if (this.dictImgsWaitForCDNAsset.Count <= 0)
		{
			return;
		}
		List<WaitForCDNAsset> list = null;
		if (this.dictImgsWaitForCDNAsset.TryGetValue(bundleIdPath, ref list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				CDNAssetBundle cdnAssetBundle = list[i].cdnAssetBundle;
				if (cdnAssetBundle != null)
				{
					cdnAssetBundle.Release();
				}
			}
			this.dictImgsWaitForCDNAsset.Remove(bundleIdPath);
		}
	}

	public WWW RequestAsset(string bundleID, Action<string, CDNAssetBundle> onComplete, Action<bool> onImgCacheCallBack = null)
	{
		CDNAssetBundle cdnassetBundle = null;
		if (this.mAssetsDic.TryGetValue(bundleID, ref cdnassetBundle))
		{
			if (cdnassetBundle.RefCount > 0)
			{
				onImgCacheCallBack = null;
				if (onComplete != null)
				{
					onComplete.Invoke(null, cdnassetBundle);
				}
				if (cdnassetBundle != null)
				{
					return cdnassetBundle.mwww;
				}
				return null;
			}
			else
			{
				this.mAssetsDic.Remove(bundleID);
			}
		}
		bool flag = false;
		CDNDownLoadData cdndownLoadData = null;
		for (int i = 0; i < this.mAssetsLoadList.Count; i++)
		{
			if (bundleID == this.mAssetsLoadList[i].assetbundleid)
			{
				cdndownLoadData = new CDNDownLoadData(bundleID, onComplete);
				this.mWaitForLoadList.Add(cdndownLoadData);
				this.mAssetsLoadList.Remove(this.mAssetsLoadList[i]);
				break;
			}
			flag = true;
		}
		if (flag)
		{
			for (int j = 0; j < this.mWaitForLoadList.Count; j++)
			{
				if (this.IsImgOrAudio(bundleID) && this.mWaitForLoadList[j].assetbundleid == bundleID)
				{
					List<Action<string, CDNAssetBundle>> list = null;
					if (this.dictActions.TryGetValue(bundleID, ref list))
					{
						List<Action<string, CDNAssetBundle>> list2 = this.dictActions[bundleID];
						list2.Add(onComplete);
					}
					else
					{
						List<Action<string, CDNAssetBundle>> list3 = new List<Action<string, CDNAssetBundle>>();
						list3.Add(onComplete);
						this.dictActions.Add(bundleID, list3);
					}
					if (onImgCacheCallBack != null)
					{
						onImgCacheCallBack.Invoke(flag);
					}
				}
			}
			if (!this.mWaitForLoadList.Exists((CDNDownLoadData result) => result.assetbundleid == bundleID))
			{
				flag = false;
				if (onImgCacheCallBack != null)
				{
					onImgCacheCallBack.Invoke(flag);
				}
			}
		}
		else if (onImgCacheCallBack != null)
		{
			onImgCacheCallBack.Invoke(flag);
		}
		if (cdndownLoadData != null)
		{
			for (int k = 0; k < this.mWaitForLoadList.Count; k++)
			{
				if (this.mWaitForLoadList[k].assetbundleid == cdndownLoadData.assetbundleid)
				{
					return this.mWaitForLoadList[k].downWWW;
				}
			}
		}
		if (!flag && onComplete != null)
		{
			MUDebug.LogError<string>(new string[]
			{
				" NOT FOUND  " + bundleID
			});
			onComplete.Invoke(bundleID + " NOT FOUND", new CDNAssetBundle(null, null));
		}
		return null;
	}

	public void SaveFiles()
	{
		if (this.isDeleting)
		{
			return;
		}
		if (this.mDownLoadList != null && this.mDownLoadList.Count > 0)
		{
			for (int i = 0; i < this.mDownLoadList.Count; i++)
			{
				if (!Global.SaveBytesToFile(PathUtils.GetPersistentPath(this.mDownLoadList[i].path), this.mDownLoadList[i].fileBytes))
				{
					MUDebug.LogError<string>(new string[]
					{
						"this file save fail : " + PathUtils.GetPersistentPath(this.mDownLoadList[i].path)
					});
				}
				else
				{
					CDNAssetBundle cdnassetBundle = null;
					if (this.mAssetsDic.TryGetValue(this.mDownLoadList[i].path, ref cdnassetBundle))
					{
						this.ReleaseImgs(this.mDownLoadList[i].path);
						cdnassetBundle.Release();
						if (cdnassetBundle.RefCount == 0)
						{
							this.mAssetsDic.Remove(this.mDownLoadList[i].path);
						}
					}
				}
			}
			this.mDownLoadList.Clear();
		}
		if (this.SaveHaveDownLoadXML())
		{
			this.currentFilesSize = 0;
			this.mCurrentDownloadCount = 0;
		}
	}

	public bool SaveHaveDownLoadXML()
	{
		if (this.mHaveDownLoadList == null || this.mHaveDownLoadList.Count <= 0)
		{
			return true;
		}
		XmlDocument xmlDocument = new XmlDocument();
		XmlNode xmlNode = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
		xmlDocument.AppendChild(xmlNode);
		XmlNode xmlNode2 = xmlDocument.CreateElement("Files");
		xmlDocument.AppendChild(xmlNode2);
		for (int i = 0; i < this.mHaveDownLoadList.Count; i++)
		{
			if (!string.IsNullOrEmpty(this.mHaveDownLoadList[i].FileName))
			{
				XElement xelement = xmlDocument.CreateElement("File");
				xelement.SetAttribute("ID", (i + 1).ToString());
				xelement.SetAttribute("Name", this.mHaveDownLoadList[i].FileName);
				xelement.SetAttribute("Size", this.mHaveDownLoadList[i].FileSize.ToString());
				xelement.SetAttribute("Type", this.mHaveDownLoadList[i].Type.ToString());
				xmlNode2.AppendChild(xelement);
			}
		}
		this.SaveErrorXML();
		return xmlDocument.Save(PathUtils.GetPersistentPath("NotInPackageAssestDwonLoadMsg.xml"));
	}

	public bool SaveErrorXML()
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlNode xmlNode = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
		xmlDocument.AppendChild(xmlNode);
		XmlNode xmlNode2 = xmlDocument.CreateElement("Files");
		xmlDocument.AppendChild(xmlNode2);
		for (int i = 0; i < this.mErrorList.Count; i++)
		{
			if (!string.IsNullOrEmpty(this.mErrorList[i].error))
			{
				XElement xelement = xmlDocument.CreateElement("File");
				xelement.SetAttribute("ID", (i + 1).ToString());
				xelement.SetAttribute("Name", this.mErrorList[i].assetBundleID);
				xelement.SetAttribute("ErrorInfo", this.mErrorList[i].error);
				xmlNode2.AppendChild(xelement);
			}
		}
		return xmlDocument.Save(PathUtils.GetPersistentPath("Error.xml"));
	}

	public void StartDownloadByExternal()
	{
		this.mCanDownload = true;
	}

	public void StopDownloadByExternal()
	{
		this.mCanDownload = false;
		this.StopDownLoadAssetCoroutine();
		this.SaveFiles();
	}

	private void Update()
	{
		if (this.IsEditorClose())
		{
			return;
		}
		if (this.mIsAllResDownload)
		{
			return;
		}
		if (Global.FenBaoType == FenBaoDownloadType.None || Global.FenBaoType == FenBaoDownloadType.Map)
		{
			return;
		}
		if (Global.FenBaoType == FenBaoDownloadType.JingMo && (int)ConfigSystemParam.GetSystemParamIntByName("AutoDown") == 0)
		{
			return;
		}
		if (this.mIsOpen && this.mCanDownload)
		{
			if (this.mCheckNetworkOnce)
			{
				this.StartWIFIUITimer();
				this.mCheckNetworkOnce = false;
			}
			if (this.mDownLoadOk && this.mChangeNet)
			{
				if (Application.internetReachability == null)
				{
					this.mDownLoadOk = false;
					this.SaveFiles();
					this.connectMaxTimes++;
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接,请检查您的网络是否正常"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
						this.mDownLoadOk = true;
						if (this.connectMaxTimes > 3)
						{
							this.connectMaxTimes = 0;
							Application.Quit();
						}
						return true;
					};
					return;
				}
				this.connectMaxTimes = 0;
				this.mDownLoadOk = false;
				CDNDownLoadData data;
				if (this.mWaitForLoadList.Count > 0)
				{
					data = this.mWaitForLoadList[0];
					this.mWaitForLoadList.RemoveAt(0);
				}
				else
				{
					if (this.mAssetsLoadList.Count <= 0)
					{
						this.SaveFiles();
						this.SaveFinishDownloadFlag();
						this.mDownLoadOk = false;
						this.mIsAllResDownload = true;
						return;
					}
					data = this.mAssetsLoadList[0];
					this.mAssetsLoadList.RemoveAt(0);
				}
				base.StartCoroutine(this.DownLoadAsset(data));
			}
		}
	}

	private IEnumerator DownLoadAsset(CDNDownLoadData data)
	{
		data.downWWW = new WWW(this.CDNPath + data.assetbundleid);
		yield return data.downWWW;
		try
		{
			if (!string.IsNullOrEmpty(data.downWWW.error))
			{
				MUDebug.LogError<string>(new string[]
				{
					data.downWWW.error
				});
				this.mAssetsLoadList.Add(data);
				if (data.onHandle != null)
				{
					data.onHandle.Invoke(data.downWWW.error, new CDNAssetBundle(null, null));
				}
				if (this.dictActions != null && this.dictActions.Count > 0)
				{
					List<Action<string, CDNAssetBundle>> listActions = null;
					if (this.dictActions.TryGetValue(data.assetbundleid, ref listActions))
					{
						for (int i = 0; i < listActions.Count; i++)
						{
							listActions[i].Invoke(data.downWWW.error, new CDNAssetBundle(null, null));
						}
						this.dictActions.Remove(data.assetbundleid);
					}
				}
				this.UpdataErrorAssetsData(data.assetbundleid, data.downWWW.error);
				this.mDownLoadOk = true;
				yield break;
			}
			if (data.downWWW.bytes.Length > 0)
			{
				DownLoadFile files = new DownLoadFile(data.assetbundleid, data.downWWW.bytes, -1);
				this.currentFilesSize += data.downWWW.bytes.Length;
				this.mDownLoadList.Add(files);
				this.mHaveDownLoadList.Add(new FileData(data.assetbundleid, (long)data.downWWW.bytes.Length, -1));
				CDNAssetBundle cdnAssetbundle = new CDNAssetBundle(data.downWWW.assetBundle, data.downWWW);
				this.mAssetsDic.Add(data.assetbundleid, cdnAssetbundle);
				if (data.onHandle != null)
				{
					data.onHandle.Invoke(data.downWWW.error, cdnAssetbundle);
				}
				if (this.dictActions != null && this.dictActions.Count > 0)
				{
					List<Action<string, CDNAssetBundle>> listActions2 = null;
					if (this.dictActions.TryGetValue(data.assetbundleid, ref listActions2))
					{
						for (int j = 0; j < listActions2.Count; j++)
						{
							listActions2[j].Invoke(data.downWWW.error, cdnAssetbundle);
						}
						this.dictActions.Remove(data.assetbundleid);
					}
				}
				this.UpdataErrorAssetsData(data.assetbundleid, null);
			}
			this.mDownLoadOk = true;
			if (this.currentFilesSize >= BackStageDownloadManager.maxFilesSize)
			{
				this.SaveFiles();
			}
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			MUDebug.LogError<Exception>(new Exception[]
			{
				ex
			});
		}
		yield break;
	}

	public bool IsImgOrAudio(string name)
	{
		return name.StartsWith("NetImages/") || name.StartsWith("Audio/");
	}

	private void UpdataErrorAssetsData(string id, string error)
	{
		ErrorAssetData errorAssetData = null;
		for (int i = 0; i < this.mErrorList.Count; i++)
		{
			if (this.mErrorList[i].assetBundleID == id)
			{
				errorAssetData = this.mErrorList[i];
				break;
			}
		}
		if (errorAssetData != null)
		{
			if (string.IsNullOrEmpty(error))
			{
				this.mErrorList.Remove(errorAssetData);
			}
			else
			{
				errorAssetData.count++;
				errorAssetData.error = string.Format("{0}|{1}", errorAssetData.error, error);
				if (errorAssetData.count > errorAssetData.MaxCount)
				{
					for (int j = 0; j < this.mAssetsLoadList.Count; j++)
					{
						if (this.mAssetsLoadList[j].assetbundleid == errorAssetData.assetBundleID)
						{
							this.mAssetsLoadList.Remove(this.mAssetsLoadList[j]);
							break;
						}
					}
					for (int k = 0; k < this.mWaitForLoadList.Count; k++)
					{
						if (this.mWaitForLoadList[k].assetbundleid == errorAssetData.assetBundleID)
						{
							this.mWaitForLoadList.Remove(this.mWaitForLoadList[k]);
							break;
						}
					}
				}
			}
		}
		else if (!string.IsNullOrEmpty(error))
		{
			this.mErrorList.Add(new ErrorAssetData(id, error, 1));
		}
	}

	private long GetDownLoadSize()
	{
		if (this.mHaveDownLoadList == null)
		{
			return 0L;
		}
		if (this.mHaveDownLoadList.Count == 0)
		{
			return 0L;
		}
		long num = 0L;
		for (int i = 0; i < this.mHaveDownLoadList.Count; i++)
		{
			num += this.mHaveDownLoadList[i].FileSize;
		}
		return num;
	}

	private void StartWIFIUITimer()
	{
		base.InvokeRepeating("WIFIUITimer_Tick", 0f, 1f);
	}

	private void StopWIFITimer()
	{
		base.CancelInvoke("WIFIUITimer_Tick");
	}

	protected void WIFIUITimer_Tick()
	{
		this.CheckNetwork();
	}

	private void CheckNetwork()
	{
		if (this.mCurrentNetState == 2 && Application.internetReachability != 2)
		{
			this.mChangeNet = false;
			long downLoadSize = this.GetDownLoadSize();
			float num = (float)(this.mNeedDLSize - downLoadSize) / 1048576f;
			if (num > 0f)
			{
				string message = string.Format(Global.GetLang("您正处在非WIFI环境，进入游戏后会自动下载游戏需求资源({0}MB)。是否继续进行游戏？"), num.ToString("0.0"));
				GChildWindow messageBoxWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), message, new Vector3(-155f, 35f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("继续游戏"), Global.GetLang("退出游戏"), 316, default(Vector3), null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						MUDebug.Log<string>(new string[]
						{
							"继续"
						});
						this.mChangeNet = true;
					}
					else
					{
						MUDebug.Log<string>(new string[]
						{
							"取消"
						});
						this.mChangeNet = false;
						Application.Quit();
					}
					return true;
				};
			}
		}
		this.mCurrentNetState = Application.internetReachability;
	}

	public bool IsNeedDownloadMapAsset(string bundleId)
	{
		bool result = false;
		if (this.mAssetsLoadList.Count > 0)
		{
			for (int i = 0; i < this.mAssetsLoadList.Count; i++)
			{
				if (this.mAssetsLoadList[i].assetbundleid == bundleId)
				{
					return true;
				}
			}
		}
		if (this.mWaitForLoadList.Count > 0)
		{
			for (int j = 0; j < this.mWaitForLoadList.Count; j++)
			{
				if (this.mWaitForLoadList[j].assetbundleid == bundleId)
				{
					return true;
				}
			}
		}
		return result;
	}

	public void SaveAssetToHaveDownLoadList(string bundleId, int bytesLength)
	{
		if (!this.mHaveDownLoadList.Exists((FileData result) => result.FileName == bundleId))
		{
			this.mHaveDownLoadList.Add(new FileData(bundleId, (long)bytesLength, -1));
			this.DeleteAssetFromWaitingDownloadList(bundleId);
		}
	}

	public void DeleteAssetFromWaitingDownloadList(string bundleId)
	{
		if (this.mAssetsLoadList.Count > 0)
		{
			for (int i = 0; i < this.mAssetsLoadList.Count; i++)
			{
				if (this.mAssetsLoadList[i].assetbundleid == bundleId)
				{
					this.mAssetsLoadList.Remove(this.mAssetsLoadList[i]);
					break;
				}
			}
		}
		if (this.mWaitForLoadList.Count > 0)
		{
			for (int j = 0; j < this.mWaitForLoadList.Count; j++)
			{
				if (this.mWaitForLoadList[j].assetbundleid == bundleId)
				{
					this.mWaitForLoadList.Remove(this.mWaitForLoadList[j]);
					break;
				}
			}
		}
	}

	public void SaveFinishDownloadFlag()
	{
		if (this.isDeleting)
		{
			return;
		}
		PlayerPrefs.SetString("FenBaoFinishDownloadRes", Context.MainExeVer);
	}

	public void DeleteFinishDownloadFlag()
	{
		this.isDeleting = true;
		this.StopDownLoadAssetCoroutine();
		this.ClearCacheRes();
		string @string = PlayerPrefs.GetString("FenBaoFinishDownloadRes");
		if (!string.IsNullOrEmpty(@string))
		{
			PlayerPrefs.SetString("FenBaoFinishDownloadRes", string.Empty);
		}
	}

	private void StopDownLoadAssetCoroutine()
	{
		base.StopCoroutine("DownLoadAsset");
	}

	public bool IsAllResDownload
	{
		get
		{
			string @string = PlayerPrefs.GetString("FenBaoFinishDownloadRes");
			return !string.IsNullOrEmpty(@string) && @string.Equals(Context.MainExeVer);
		}
	}

	private void ClearCacheRes()
	{
		this.mAssetsLoadList.Clear();
		this.mWaitForLoadList.Clear();
		this.mHaveDownLoadList.Clear();
	}

	private static readonly int maxFilesSize = 10240000;

	public static BackStageDownloadManager instance;

	private int currentFilesSize;

	private int mCurrentDownloadCount;

	private int mMAXDownloadCount = 8;

	[SerializeField]
	private bool mDownLoadOk;

	[SerializeField]
	private bool mCanDownload = true;

	private string CDNPath = string.Empty;

	private List<DownLoadFile> mDownLoadList = new List<DownLoadFile>();

	private List<FileData> mHaveDownLoadList = new List<FileData>();

	private List<CDNDownLoadData> mAssetsLoadList = new List<CDNDownLoadData>();

	private List<CDNDownLoadData> mWaitForLoadList = new List<CDNDownLoadData>();

	private Dictionary<string, CDNAssetBundle> mAssetsDic = new Dictionary<string, CDNAssetBundle>();

	private List<ErrorAssetData> mErrorList = new List<ErrorAssetData>();

	private int connectMaxTimes;

	private NetworkReachability mCurrentNetState = 2;

	private long mNeedDLSize;

	private bool mChangeNet;

	private bool mIsOpen;

	private bool mtIsAllResDownload;

	private Dictionary<string, List<WaitForCDNAsset>> dictImgsWaitForCDNAsset = new Dictionary<string, List<WaitForCDNAsset>>();

	private Dictionary<string, List<Action<string, CDNAssetBundle>>> dictActions = new Dictionary<string, List<Action<string, CDNAssetBundle>>>();

	private bool mCheckNetworkOnce = true;

	private bool isDeleting;

	[StructLayout(0, Size = 1)]
	public struct CfgData
	{
		public string name { get; set; }

		public long size { get; set; }

		public int type { get; set; }
	}
}
