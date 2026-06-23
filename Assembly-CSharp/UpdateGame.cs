using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using HSGameEngine.JavaPlugins;
using Tmsk.Xml;
using UnityEngine;

public class UpdateGame : UserControl
{
	public int LocalAppVerCode { get; set; }

	public int RemoteAppVerCode { get; set; }

	public string URL { get; set; }

	public byte[] RemoteVersionBytes { get; set; }

	private void InitPrefabText()
	{
		try
		{
		}
		catch
		{
			MUDebug.LogError<string>(new string[]
			{
				"YN Debug:" + base.GetType().Name + "_InitPrefabText(),has a error,please check!"
			});
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		if (null != this.MapPic)
		{
			this.MapPic.URL = "LoadGame/11.jpg";
		}
	}

	public void ShowUpdateGameInfo()
	{
		try
		{
			this.NeedDownSizeValue = Math.Round((double)(this.GetTotalNeedDownloadSize() / 1024f / 1024f), 2).ToString("0.0");
		}
		catch
		{
			MUDebug.LogError<string>(new string[]
			{
				"YN_Debug:ShowUpdateGameInfo has a error,please check!"
			});
		}
		if (!QMQJJava.CheckSDCard())
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetLang("当前为不读取SD卡模式,进入游戏将下载大量资源,是否确认游戏?"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					this.StartCoroutine<bool>(this.InitUpdate());
				}
				else if (messageBoxReturn == 1)
				{
					Application.Quit();
				}
				return true;
			};
		}
		else
		{
			base.StartCoroutine<bool>(this.InitUpdate());
		}
	}

	public void ShowUpdateZIPGameInfo()
	{
		try
		{
			this.NeedDownSizeValue = Math.Round((double)(this.GetTotalNeedDownloadZIPSize() / 1024f / 1024f), 2).ToString("0.0");
		}
		catch
		{
			MUDebug.LogError<string>(new string[]
			{
				"YN_Debug:ShowUpdateZIPGameInfo has a error,please check!"
			});
		}
		if (!QMQJJava.CheckSDCard())
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetLang("当前为不读取SD卡模式,进入游戏将下载大量资源,是否确认游戏?"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					this.StartCoroutine<bool>(this.InitZIPUpdate());
				}
				else if (messageBoxReturn == 1)
				{
					Application.Quit();
				}
				return true;
			};
		}
		else
		{
			base.StartCoroutine<bool>(this.InitZIPUpdate());
		}
	}

	public override void Destroy()
	{
		MainGame._current.CancelCheckHasNetwork();
		base.Destroy();
	}

	public static IEnumerator InitConfigVersion()
	{
		Super.ShowCheckingUpdateGame(MainGame._current.Stage);
		yield return new WaitForSeconds(4.5f);
		if (Application.internetReachability == null)
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接,请检查您的网络是否正常！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				Application.Quit();
				return true;
			};
			yield break;
		}
		MainGame._current.StartCheckHasNetwork();
		WWW www = null;
		XElement xml = null;
		string persistentXMLFile = null;
		string content = null;
		MUDebug.Log<string>(new string[]
		{
			"InitConfigVersion 1:" + Time.realtimeSinceStartup
		});
		persistentXMLFile = PathUtils.GetPersistentPath("version.xml");
		MUDebug.Log<string>(new string[]
		{
			"越南测试用UpdateGame_persistentXMLFile=" + persistentXMLFile + ";persistentXMLFile=" + persistentXMLFile
		});
		if (File.Exists(persistentXMLFile))
		{
			www = new WWW(PathUtils.GetWWWPath(persistentXMLFile));
			MUDebug.Log<string>(new string[]
			{
				"越南测试用UpdateGame_if(File.Exists(persistentXMLFile))_www.url=" + www.url
			});
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				MUDebug.LogError<string>(new string[]
				{
					"越南测试用UpdateGame_if(File.Exists(persistentXMLFile))_if (!string.IsNullOrEmpty(www.error))_www.error=" + www.error
				});
			}
			else
			{
				content = Global.GetUTF8StringFromBytes(Program.DecryptSceneData(www.bytes));
				content = GameConfigManager.ParseProgramData(content);
				xml = XElement.Parse(content);
				MUDebug.LogError<string>(new string[]
				{
					"越南测试用UpdateGame_if(File.Exists(persistentXMLFile))_if (!string.IsNullOrEmpty(www.error))else_content" + content
				});
				if (xml != null)
				{
					Global.PersistentVersionXML = xml;
					MUDebug.Log<string>(new string[]
					{
						"越南测试用UpdateGame_if(File.Exists(persistentXMLFile))_else_if(xml != null)_PersistentVersionXML=" + Global.PersistentVersionXML
					});
				}
			}
			MUDebug.Log<string>(new string[]
			{
				"越南测试用UpdateGame_if(File.Exists(persistentXMLFile))_执行www.Dispose();"
			});
			www.Dispose();
		}
		MUDebug.Log<string>(new string[]
		{
			"InitConfigVersion 2:" + Time.realtimeSinceStartup
		});
		float starttime = Time.realtimeSinceStartup;
		www = new WWW(PathUtils.SteamingAssetsPath_DontUseThis("version.xml"));
		MUDebug.Log<string>(new string[]
		{
			"越南测试用UpdateGame_www.url=" + www.url
		});
		yield return www;
		MUDebug.Log<string>(new string[]
		{
			"越南测试用UpdateGame_yield return www_end"
		});
		if (!string.IsNullOrEmpty(www.error))
		{
			MUDebug.LogError<string>(new string[]
			{
				"越南测试用UpdateGame_line255_www.error=" + www.error
			});
			MUDebug.LogError<string>(new string[]
			{
				"获取安装包中version.xml错误"
			});
			yield break;
		}
		content = Global.GetUTF8StringFromBytes(Program.DecryptSceneData(www.bytes));
		content = GameConfigManager.ParseProgramData(content);
		MUDebug.Log<string>(new string[]
		{
			"越南测试用UpdateGame_content=" + content
		});
		try
		{
			xml = XElement.Parse(content);
		}
		catch
		{
			MUDebug.LogError<string>(new string[]
			{
				"越南测试用UpdateGame_XElement.Parse(content)解析出错！content=" + content
			});
		}
		MUDebug.Log<string>(new string[]
		{
			"越南测试用UpdateGame_line271_xml.Name=" + xml.Name
		});
		if (xml == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"安装包中Version.xml错误"
			});
			yield break;
		}
		MUDebug.Log<string>(new string[]
		{
			"执行www.Dispose();"
		});
		www.Dispose();
		Global.VersionXml = xml;
		MUDebug.Log<string>(new string[]
		{
			"越南测试用Global.VersionXml=" + Global.VersionXml
		});
		Global.IsMiniPackage = (Global.ReadXmlConfigStr(Global.VersionXml, "PackageType", "IsMiniPack") == "true");
		Global.FenBaoType = (FenBaoDownloadType)Global.ReadXmlConfigInt(Global.VersionXml, "FenBaoType", "IsFenBao");
		MUDebug.Log<string>(new string[]
		{
			"IsMiniPackage" + Global.IsMiniPackage
		});
		MUDebug.Log<string>(new string[]
		{
			"IsFenBao " + (int)Global.FenBaoType
		});
		MUDebug.Log<string>(new string[]
		{
			"InitConfigVersion 3:" + Time.realtimeSinceStartup
		});
		string url = null;
		url = Global.ReadXmlConfigStr(Global.VersionXml, "Info", "versionurl");
		MUDebug.Log<string>(new string[]
		{
			"越南测试用UpdateGame.cs_line297_url=" + url
		});
		if (!url.StartsWith("http://") && !url.StartsWith("https://"))
		{
			url = "https://" + url;
		}
		if (PlatSDKMgr.PlatName == "TM_TEST")
		{
			url = url.Trim().Trim(new char[]
			{
				'/'
			}) + "/TMLogin/";
		}
		else
		{
			url = url.Trim().Trim(new char[]
			{
				'/'
			}) + "/" + PlatSDKMgr.PlatName + "Login/";
		}
		url = PathUtils.GetWWWPath(url + "version.xml") + "?v=" + Global.GetTimeStamp();
		MUDebug.Log<string>(new string[]
		{
			"越南测试用UpdateGame.cs_line312_url=" + url
		});
		www = new WWW(url);
		MUDebug.Log<string>(new string[]
		{
			"越南测试用UpdateGame.cs_line314_www.url=" + www.url
		});
		UpdateGame.StartTimer();
		yield return www;
		UpdateGame.StopTimer();
		if (!string.IsNullOrEmpty(www.error))
		{
			MUDebug.LogError<string>(new string[]
			{
				www.error
			});
			url = Global.ReadXmlConfigStr(Global.VersionXml, "Info", "bakurl");
			url = url.Trim().Trim(new char[]
			{
				'/'
			}) + "/";
			url = PathUtils.GetWWWPath(url + "version.xml") + "?v=" + Global.GetTimeStamp();
			www = new WWW(url);
			MUDebug.Log<string>(new string[]
			{
				www.url
			});
			UpdateGame.StartTimer();
			yield return www;
			UpdateGame.StopTimer();
			if (!string.IsNullOrEmpty(www.error))
			{
				MUDebug.LogError<string>(new string[]
				{
					www.error
				});
				GChildWindow messageBoxWindow2 = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接(version),请检查您的网络是否正常！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow2.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow2.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow2);
					Application.Quit();
					return true;
				};
				yield break;
			}
		}
		MUDebug.Log<string>(new string[]
		{
			"越南测试用UpdateGame.cs_if (!string.IsNullOrEmpty(www.error))url地址正确——结束开始解析Program.DecryptSceneData(www.bytes)"
		});
		UpdateGame.remoteVersionBytes = www.bytes;
		MUDebug.Log<string>(new string[]
		{
			"越南测试用UpdateGame.cs_开始解析Use Program.DecryptSceneData"
		});
		content = Global.GetUTF8StringFromBytes(Program.DecryptSceneData(www.bytes));
		content = GameConfigManager.ParseProgramData(content);
		MUDebug.Log<string>(new string[]
		{
			"越南测试用UpdateGame.cs_线上版本Version：" + content
		});
		try
		{
			xml = XElement.Parse(content);
		}
		catch
		{
			MUDebug.LogError<string>(new string[]
			{
				"越南测试用UpdateGame_XElement.Parse(content)解析出错！content=" + content
			});
		}
		www.Dispose();
		if (xml == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"没有找到远程的version.xml"
			});
			yield break;
		}
		Global.NetVersionXML = xml;
		int localResVersion = Global.ReadXmlConfigInt(Global.VersionXml, "Resource", "VerCode");
		int netResVersion = Global.ReadXmlConfigInt(Global.NetVersionXML, "Resource", "VerCode");
		MUDebug.Log<string>(new string[]
		{
			"越南测试用手机本地localResVersion:" + localResVersion
		});
		MUDebug.Log<string>(new string[]
		{
			"越南测试用服务器端netResVersion:" + netResVersion
		});
		bool isInnerVerify = false;
		if (Global.PersistentVersionXML != null)
		{
			isInnerVerify = (Global.ReadXmlConfigInt(Global.PersistentVersionXML, "Info", "IsInner") == 1);
		}
		if (localResVersion > netResVersion || isInnerVerify)
		{
			url = Global.ReadXmlConfigStr(Global.NetVersionXML, "Test", "TestURL");
			url = url.Trim().Trim(new char[]
			{
				'/'
			}) + "/";
			url = PathUtils.GetWWWPath(url + "version.xml") + "?v=" + Global.GetTimeStamp();
			www = new WWW(url);
			MUDebug.Log<string>(new string[]
			{
				www.url
			});
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				MUDebug.LogError<string>(new string[]
				{
					www.error
				});
				GChildWindow messageBoxWindow3 = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接(TestURL Version),请检查您的网络是否正常！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow3.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow3.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow3);
					Application.Quit();
					return true;
				};
				yield break;
			}
			UpdateGame.remoteVersionBytes = www.bytes;
			MUDebug.Log<string>(new string[]
			{
				"Use Program.DecryptSceneData"
			});
			content = Global.GetUTF8StringFromBytes(Program.DecryptSceneData(www.bytes));
			content = GameConfigManager.ParseProgramData(content);
			MUDebug.Log<string>(new string[]
			{
				"越南测试用UpdateGame.cs_line419_content:" + content
			});
			try
			{
				xml = XElement.Parse(content);
			}
			catch
			{
				MUDebug.LogError<string>(new string[]
				{
					"越南测试用UpdateGame_XElement.Parse(content)解析出错！content=" + content
				});
			}
			www.Dispose();
			if (xml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"没有找到远程的version.xml"
				});
				yield break;
			}
			Global.NetVersionXML = xml;
		}
		int localLevelupVersion = Global.ReadXmlConfigInt(Global.VersionXml, "AppLevelUp", "VerCode");
		int netLevelupVersion = Global.ReadXmlConfigInt(Global.NetVersionXML, "AppLevelUp", "VerCode");
		if (localLevelupVersion != -1 && netLevelupVersion != -1)
		{
			int isLevelUpOpen = Global.ReadXmlConfigInt(Global.NetVersionXML, "AppLevelUp", "isOpen");
			if (isLevelUpOpen == 1 && localLevelupVersion < netLevelupVersion)
			{
				string linkURL = Global.ReadXmlConfigStr(Global.NetVersionXML, "AppLevelUp", "linkurl");
				string message = Global.ReadXmlConfigStr(Global.NetVersionXML, "AppLevelUp", "msg");
				GChildWindow messageBoxWindow4 = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("公告"), message, -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow4.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow4.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow4);
					if (messageBoxReturn == 0)
					{
						if (!string.IsNullOrEmpty(linkURL))
						{
							Application.OpenURL(linkURL);
						}
						Application.Quit();
					}
					else if (messageBoxReturn == 1)
					{
						Application.Quit();
					}
					return true;
				};
				yield break;
			}
		}
		int isZipFromNet = Global.ReadXmlConfigInt(Global.NetVersionXML, "Info", "isZip");
		if (LocalStorage.ExistKey("IsMiniPackZipUpdated"))
		{
			Global.IsMiniPackZipUpdated = (LocalStorage.GetString("IsMiniPackZipUpdated").ToLower() == "true");
			MUDebug.Log<string>(new string[]
			{
				"LocalStorage IsMiniPackZipUpdated = " + Global.IsMiniPackZipUpdated
			});
		}
		if (Global.IsMiniPackage && isZipFromNet == 1 && !Global.IsMiniPackZipUpdated)
		{
			MainGame._current.StartCoroutine<bool>(UpdateGame.CheckZipIndex());
		}
		else
		{
			MainGame._current.StartCoroutine<bool>(UpdateGame.CheckFileIndex());
		}
		yield break;
	}

	public static IEnumerator CheckFileIndex()
	{
		WWW www = null;
		XElement xml = null;
		string content = null;
		string url = null;
		UpdateGame.UpdatePackageName = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "updateclientname");
		if (UpdateGame.UpdatePackageName == null || UpdateGame.UpdatePackageName == string.Empty)
		{
			UpdateGame.UpdatePackageName = "ClientUpdate.apk";
		}
		MUDebug.Log<string>(new string[]
		{
			"UpdatePackageName:" + UpdateGame.UpdatePackageName
		});
		MUDebug.Log<string>(new string[]
		{
			"InitConfigVersion version over:" + Time.realtimeSinceStartup
		});
		int persistentResCode = -1;
		int persistentAppVerCode = -1;
		if (Global.PersistentVersionXML != null)
		{
			persistentResCode = Global.ReadXmlConfigInt(Global.PersistentVersionXML, "Resource", "VerCode");
			persistentAppVerCode = Global.ReadXmlConfigInt(Global.PersistentVersionXML, "Application", "VerCode");
		}
		int assetsStreamingResCode = Global.ReadXmlConfigInt(Global.VersionXml, "Resource", "VerCode");
		int assetsStreamingAppCode = Global.ReadXmlConfigInt(Global.VersionXml, "Application", "VerCode");
		int remoteResCode = Global.ReadXmlConfigInt(Global.NetVersionXML, "Resource", "VerCode");
		int remoteAppVerCode = Global.ReadXmlConfigInt(Global.NetVersionXML, "Application", "VerCode");
		MUDebug.Log<string>(new string[]
		{
			string.Format("persistentResCode={0}, persistentAppVerCode={1}, assetsStreamingResCode={2}, assetsStreamingResCode={3}, assetsStreamingAppCode={4}, remoteResCode={5}", new object[]
			{
				persistentResCode,
				persistentAppVerCode,
				assetsStreamingResCode,
				assetsStreamingResCode,
				assetsStreamingAppCode,
				remoteResCode
			})
		});
		bool showUpdateGameForRes = UpdateGame.IsNeedShowUpdateGameForRes(persistentResCode, remoteResCode);
		Context.MainExeVer = assetsStreamingAppCode.ToString();
		Context.ResSwfVer = remoteResCode.ToString();
		Context.IsAPPVerify = (Global.ReadXmlConfigInt(Global.NetVersionXML, "Info", "IsAppVerify") == 1);
		if (File.Exists(PathUtils.GetPersistentPath("ios.app")))
		{
			Context.IsAPPVerifyC = true;
			Context.IsAPPVerify = true;
		}
		if (showUpdateGameForRes || assetsStreamingAppCode < remoteAppVerCode)
		{
			MUDebug.Log<string>(new string[]
			{
				"InitConfigVersion  need update:" + Time.realtimeSinceStartup
			});
			string persistentIndexXMLFile = PathUtils.GetPersistentPath("index.xml");
			MUDebug.Log<string>(new string[]
			{
				persistentIndexXMLFile
			});
			if (File.Exists(persistentIndexXMLFile))
			{
				www = new WWW(PathUtils.GetWWWPath(persistentIndexXMLFile));
				MUDebug.Log<string>(new string[]
				{
					www.url
				});
				yield return www;
				if (!string.IsNullOrEmpty(www.error))
				{
					MUDebug.LogError<string>(new string[]
					{
						www.error
					});
					MUDebug.LogError<string>(new string[]
					{
						persistentIndexXMLFile + Global.GetLang("错误")
					});
					yield break;
				}
				MUDebug.Log<string>(new string[]
				{
					"Save to" + persistentIndexXMLFile
				});
				content = Global.GetUTF8StringFromBytes(www.bytes);
				xml = XElement.Parse(content);
				if (xml != null)
				{
					Global.PersistentIndexXML = xml;
				}
				www.Dispose();
			}
			MUDebug.Log<string>(new string[]
			{
				"InitConfigVersion  need update 2:" + Time.realtimeSinceStartup
			});
			www = new WWW(PathUtils.SteamingAssetsPath_DontUseThis("index.xml"));
			MUDebug.Log<string>(new string[]
			{
				www.url
			});
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				MUDebug.LogError<string>(new string[]
				{
					www.error
				});
				MUDebug.LogError<string>(new string[]
				{
					"安装包中index.xml错误"
				});
				yield break;
			}
			content = Global.GetUTF8StringFromBytes(www.bytes);
			xml = XElement.Parse(content);
			if (xml != null)
			{
				Global.IndexXML = xml;
			}
			www.Dispose();
			if (Global.NetVersionXML != null)
			{
				string realIndex = "index.xml";
				if (Global.FenBaoType == FenBaoDownloadType.TuiGuang)
				{
					realIndex = "index_TuiGuang.xml";
					MUDebug.LogError<string>(new string[]
					{
						"===index_TuiGuang.xmljindex下载"
					});
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						"===正常Index.xml下载"
					});
				}
				url = Global.ReadXmlConfigStr(Global.NetVersionXML, string.Empty, "URL");
				url = url.Trim().Trim(new char[]
				{
					'/'
				}) + "/";
				www = new WWW(PathUtils.GetWWWPath(url + realIndex) + "?v=" + Global.GetTimeStamp());
				MUDebug.Log<string>(new string[]
				{
					www.url
				});
				yield return www;
				if (!string.IsNullOrEmpty(www.error))
				{
					MUDebug.LogError<string>(new string[]
					{
						www.error
					});
					url = Global.ReadXmlConfigStr(Global.NetVersionXML, string.Empty, "URL");
					url = url.Trim().Trim(new char[]
					{
						'/'
					}) + "/";
					www = new WWW(PathUtils.GetWWWPath(url + realIndex) + "?v=" + Global.GetTimeStamp());
					if (!string.IsNullOrEmpty(www.error))
					{
						MUDebug.LogError<string>(new string[]
						{
							www.error
						});
						GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接,请检查您的网络是否正常！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
							Application.Quit();
							return true;
						};
						yield break;
					}
				}
				while (www.bytes.Length == 0)
				{
					yield return new WaitForSeconds(0.05f);
				}
				content = Global.GetUTF8StringFromBytes(www.bytes);
				xml = XElement.Parse(content);
				if (xml != null)
				{
					Global.NetIndexXML = xml;
				}
				MUDebug.Log<string>(new string[]
				{
					"Global.NetIndexXML:" + Global.NetIndexXML
				});
				www.Dispose();
			}
			MUDebug.Log<string>(new string[]
			{
				"InitConfigVersion  need update 3:" + Time.realtimeSinceStartup
			});
			List<XElement> needToUpdateFileList = new List<XElement>();
			List<string> needToDeleteFileList = new List<string>();
			UpdateGame.InitUpdateResource(needToUpdateFileList, needToDeleteFileList);
			MUDebug.Log<string>(new string[]
			{
				"InitConfigVersion  need update 4:" + Time.realtimeSinceStartup
			});
			if (Global.PersistentIndexXML == null)
			{
				XmlDocument xmlDoc = new XmlDocument();
				XmlDeclaration dec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
				xmlDoc.AppendChild(dec);
				XElement xmlFiles = xmlDoc.CreateElement("Files");
				xmlDoc.AppendChild(xmlFiles);
				xmlDoc.Save(persistentIndexXMLFile);
				www = new WWW(PathUtils.GetWWWPath(persistentIndexXMLFile));
				yield return www;
				if (!string.IsNullOrEmpty(www.error))
				{
					MUDebug.LogError<string>(new string[]
					{
						www.error
					});
					MUDebug.LogError<string>(new string[]
					{
						"获取" + PathUtils.GetWWWPath(persistentIndexXMLFile) + Global.GetLang("错误")
					});
					yield break;
				}
				content = Global.GetUTF8StringFromBytes(www.bytes);
				xml = XElement.Parse(content);
				if (xml != null)
				{
					Global.PersistentIndexXML = xml;
				}
				www.Dispose();
			}
			MUDebug.Log<string>(new string[]
			{
				"YN_Debug:UpdateGame_begin_Super.ShowUpDateCDNTip()"
			});
			Super.DestroyCheckingUpdateGame();
			Super.ShowUpDateCDNTip(MainGame._current.Stage, assetsStreamingAppCode, remoteAppVerCode, Global.ReadXmlConfigStr(Global.NetVersionXML, string.Empty, "URL"), UpdateGame.remoteVersionBytes, needToUpdateFileList, needToDeleteFileList);
			MUDebug.Log<string>(new string[]
			{
				"YN_Debug:UpdateGame_Super.ShowUpDateCDNTip()_end"
			});
		}
		else
		{
			if (Time.realtimeSinceStartup - UpdateGame.starttime < 1.5f)
			{
				yield return new WaitForSeconds(1.5f - Time.realtimeSinceStartup + UpdateGame.starttime);
			}
			Super.ShowLoadingGame(MainGame._current.Stage);
			Super.DestroyCheckingUpdateGame();
			MainGame._current.CancelCheckHasNetwork();
		}
		MUDebug.Log<string>(new string[]
		{
			"======null != UpdateNextStep=====" + (null != UpdateGame.UpdateNextStep)
		});
		if (UpdateGame.UpdateNextStep != null)
		{
			MUDebug.Log<string>(new string[]
			{
				"======null != UpdateNextStep===== 1111111111111111"
			});
			UpdateGame.UpdateNextStep(null, new NextStepEventArgs
			{
				StepType = 0
			});
		}
		yield break;
	}

	public static IEnumerator CheckZipIndex()
	{
		WWW www = null;
		XElement xml = null;
		string content = null;
		string url = string.Empty;
		string persistentZIPIndexXMLFile = PathUtils.GetPersistentPath("ZIPIndex.xml");
		MUDebug.Log<string>(new string[]
		{
			persistentZIPIndexXMLFile
		});
		if (File.Exists(persistentZIPIndexXMLFile))
		{
			www = new WWW(PathUtils.GetWWWPath(persistentZIPIndexXMLFile));
			MUDebug.Log<string>(new string[]
			{
				www.url
			});
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				MUDebug.LogError<string>(new string[]
				{
					www.error
				});
				MUDebug.LogError<string>(new string[]
				{
					persistentZIPIndexXMLFile + Global.GetLang("错误")
				});
				yield break;
			}
			content = Global.GetUTF8StringFromBytes(www.bytes);
			xml = XElement.Parse(content);
			if (xml != null)
			{
				Global.PersistentZIPIndexXML = xml;
			}
			www.Dispose();
		}
		if (Global.NetVersionXML != null)
		{
			url = Global.ReadXmlConfigStr(Global.NetVersionXML, string.Empty, "URL");
			url = url.Trim().Trim(new char[]
			{
				'/'
			}) + "/";
			www = new WWW(PathUtils.GetWWWPath(url + "ZIPIndex.xml") + "?v=" + Global.GetTimeStamp());
			MUDebug.Log<string>(new string[]
			{
				www.url
			});
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				MUDebug.LogError<string>(new string[]
				{
					www.error
				});
				url = Global.ReadXmlConfigStr(Global.VersionXml, "Info", "bakurl");
				url = url.Trim().Trim(new char[]
				{
					'/'
				}) + "/";
				www = new WWW(PathUtils.GetWWWPath(url + "ZIPIndex.xml") + "?v=" + Global.GetTimeStamp());
				if (!string.IsNullOrEmpty(www.error))
				{
					MUDebug.LogError<string>(new string[]
					{
						www.error
					});
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接(bakurl),请检查您的网络是否正常！") + www.error, -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
						Application.Quit();
						return true;
					};
					yield break;
				}
			}
			while (www.bytes.Length == 0)
			{
				yield return new WaitForSeconds(0.05f);
			}
			content = Global.GetUTF8StringFromBytes(www.bytes);
			xml = XElement.Parse(content);
			if (xml != null)
			{
				Global.NetZIPIndexXML = xml;
			}
			MUDebug.Log<string>(new string[]
			{
				"Global.NetZIPIndexXML:" + Global.NetZIPIndexXML
			});
			www.Dispose();
		}
		List<XElement> needToUpdateZipFileList = new List<XElement>();
		List<string> needToDeleteZipFileList = new List<string>();
		UpdateGame.InitUpdateZIPResource(needToUpdateZipFileList, needToDeleteZipFileList);
		if (Global.PersistentZIPIndexXML == null)
		{
			XmlDocument xmlDoc = new XmlDocument();
			XmlDeclaration dec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
			xmlDoc.AppendChild(dec);
			XElement xmlFiles = xmlDoc.CreateElement("Files");
			xmlDoc.AppendChild(xmlFiles);
			xmlDoc.Save(persistentZIPIndexXMLFile);
			www = new WWW(PathUtils.GetWWWPath(persistentZIPIndexXMLFile));
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				MUDebug.LogError<string>(new string[]
				{
					www.error
				});
				MUDebug.LogError<string>(new string[]
				{
					"获取" + PathUtils.GetWWWPath(persistentZIPIndexXMLFile) + "错误"
				});
				yield break;
			}
			content = Global.GetUTF8StringFromBytes(www.bytes);
			xml = XElement.Parse(content);
			if (xml != null)
			{
				Global.PersistentZIPIndexXML = xml;
			}
			www.Dispose();
		}
		MUDebug.Log<string>(new string[]
		{
			"InitConfigVersion  need update 5:" + Time.realtimeSinceStartup
		});
		Super.DestroyCheckingUpdateGame();
		Super.ShowUpDateZIPTip(MainGame._current.Stage, Global.ReadXmlConfigStr(Global.NetVersionXML, string.Empty, "URL"), needToUpdateZipFileList, needToDeleteZipFileList);
		yield break;
	}

	private static bool IsNeedShowUpdateGameForRes(int persistentResCode, int netResCode)
	{
		bool result = true;
		if (netResCode > persistentResCode)
		{
			return true;
		}
		if (LocalStorage.ExistKey("IsMiniPackUpdated"))
		{
			Global.IsMiniPackUpdated = (LocalStorage.GetString("IsMiniPackUpdated").ToLower() == "true");
			MUDebug.Log<string>(new string[]
			{
				"LocalStorage IsMiniPackUpdated = " + Global.IsMiniPackUpdated
			});
		}
		Global.IsFullPackUpdated = false;
		if (LocalStorage.ExistKey("IsFullPackUpdatedRes"))
		{
			int num = Global.ReadXmlConfigInt(Global.VersionXml, "Resource", "VerCode");
			Global.IsFullPackUpdated = (LocalStorage.GetString("IsFullPackUpdatedRes") == num.ToString());
			MUDebug.Log<string>(new string[]
			{
				"LocalStorage IsFullPackUpdatedRes = " + Global.IsFullPackUpdated
			});
		}
		if (LocalStorage.ExistKey("IsFullPackUpdatedApp"))
		{
			int num2 = Global.ReadXmlConfigInt(Global.VersionXml, "Application", "VerCode");
			bool flag = LocalStorage.GetString("IsFullPackUpdatedApp") == num2.ToString();
			Global.IsFullPackUpdated = (Global.IsFullPackUpdated && flag);
			MUDebug.Log<string>(new string[]
			{
				"LocalStorage IsFullPackUpdatedApp && IsFullPackUpdatedRes " + Global.IsFullPackUpdated
			});
		}
		if (Global.IsMiniPackage)
		{
			if (Global.IsMiniPackUpdated)
			{
				result = false;
			}
		}
		else if (Global.IsFullPackUpdated)
		{
			result = false;
		}
		return result;
	}

	private static void InitUpdateResource(List<XElement> needToUpdateFileList, List<string> needToDeleteFileList)
	{
		Dictionary<string, XElement> dictionary = new Dictionary<string, XElement>();
		List<XElement> xelementList = Global.GetXElementList(Global.IndexXML, "File");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
			dictionary[xelementAttributeStr] = xelement;
		}
		Dictionary<string, XElement> dictionary2 = new Dictionary<string, XElement>();
		if (Global.PersistentIndexXML != null)
		{
			List<XElement> xelementList2 = Global.GetXElementList(Global.PersistentIndexXML, "File");
			for (int j = 0; j < xelementList2.Count; j++)
			{
				XElement xelement2 = xelementList2[j];
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "Name");
				dictionary2[xelementAttributeStr] = xelement2;
			}
		}
		List<XElement> xelementList3 = Global.GetXElementList(Global.NetIndexXML, "File");
		int num = Global.ReadXmlConfigInt(Global.VersionXml, "Resource", "VerCode");
		int num2 = Global.ReadXmlConfigInt(Global.PersistentVersionXML, "Resource", "VerCode");
		int num3 = Global.ReadXmlConfigInt(Global.NetVersionXML, "Resource", "VerCode");
		for (int k = 0; k < xelementList3.Count; k++)
		{
			XElement xelement3 = xelementList3[k];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement3, "Name");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement3, "FileSize");
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement3, "CRC32");
			string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement3, "Name");
			if (xelementAttributeStr4.IndexOf("version.xml") < 0 && xelementAttributeStr4.IndexOf("index.xml") < 0)
			{
				XElement xelement4 = null;
				if (Global.IsMiniPackage)
				{
					if (dictionary2.TryGetValue(xelementAttributeStr, ref xelement4))
					{
						string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement4, "FileSize");
						string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement4, "CRC32");
						if (xelementAttributeStr2 != xelementAttributeStr5 || xelementAttributeStr3 != xelementAttributeStr6)
						{
							needToDeleteFileList.Add(xelementAttributeStr);
							needToUpdateFileList.Add(xelement3);
						}
					}
					else
					{
						needToUpdateFileList.Add(xelement3);
					}
				}
				else if (num > num2 && num2 != -1)
				{
					if (dictionary.TryGetValue(xelementAttributeStr, ref xelement4))
					{
						string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement4, "FileSize");
						string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement4, "CRC32");
						if (xelementAttributeStr2 != xelementAttributeStr5 || xelementAttributeStr3 != xelementAttributeStr6)
						{
							needToUpdateFileList.Add(xelement3);
						}
					}
					else
					{
						needToUpdateFileList.Add(xelement3);
					}
					if (dictionary2.TryGetValue(xelementAttributeStr, ref xelement4))
					{
						needToDeleteFileList.Add(xelementAttributeStr);
					}
				}
				else if (dictionary2.TryGetValue(xelementAttributeStr, ref xelement4))
				{
					string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement4, "FileSize");
					string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement4, "CRC32");
					if (xelementAttributeStr2 != xelementAttributeStr5 || xelementAttributeStr3 != xelementAttributeStr6)
					{
						needToUpdateFileList.Add(xelement3);
						needToDeleteFileList.Add(xelementAttributeStr);
					}
				}
				else if (dictionary.TryGetValue(xelementAttributeStr, ref xelement4))
				{
					string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement4, "FileSize");
					string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement4, "CRC32");
					if (xelementAttributeStr2 != xelementAttributeStr5 || xelementAttributeStr3 != xelementAttributeStr6)
					{
						needToUpdateFileList.Add(xelement3);
					}
				}
				else
				{
					needToUpdateFileList.Add(xelement3);
				}
			}
		}
	}

	private static void InitUpdateZIPResource(List<XElement> needToUpdateZIPList, List<string> needToDeleteZIPList)
	{
		Dictionary<string, XElement> dictionary = new Dictionary<string, XElement>();
		if (Global.PersistentZIPIndexXML != null)
		{
			List<XElement> xelementList = Global.GetXElementList(Global.PersistentZIPIndexXML, "File");
			foreach (XElement xelement in xelementList)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
				dictionary[xelementAttributeStr] = xelement;
			}
		}
		List<XElement> xelementList2 = Global.GetXElementList(Global.NetZIPIndexXML, "File");
		foreach (XElement xelement2 in xelementList2)
		{
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "Name");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "FileSize");
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "CRC32");
			string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement2, "Name");
			XElement xelement3 = null;
			if (dictionary.TryGetValue(xelementAttributeStr, ref xelement3))
			{
				string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement3, "FileSize");
				string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement3, "CRC32");
				if (xelementAttributeStr2 != xelementAttributeStr5 || xelementAttributeStr3 != xelementAttributeStr6)
				{
					needToDeleteZIPList.Add(xelementAttributeStr);
					needToUpdateZIPList.Add(xelement2);
				}
			}
			else
			{
				needToUpdateZIPList.Add(xelement2);
			}
		}
	}

	public void SetProcessText(string text)
	{
		this.progressText.text = text;
	}

	private void InitDeleteItem()
	{
		if (this.NeedToDeleteFileList != null && this.NeedToDeleteFileList.Count > 0 && Global.PersistentIndexXML != null)
		{
			MUDebug.Log<string>(new string[]
			{
				"InitDeleteItem Count:" + this.NeedToDeleteFileList.Count
			});
			XElement xelement = null;
			Dictionary<string, XElement> dictionary = new Dictionary<string, XElement>();
			List<XElement> xelementList = Global.GetXElementList(Global.PersistentIndexXML, "File");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement2 = xelementList[i];
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "Name");
				dictionary[xelementAttributeStr] = xelement2;
			}
			this.HintUser1.text = Global.GetLang("提示：正在清理冗余资源");
			for (int j = 0; j < this.NeedToDeleteFileList.Count; j++)
			{
				string text = this.NeedToDeleteFileList[j];
				if (dictionary.TryGetValue(text, ref xelement))
				{
					string text2 = Global.GetXElementAttributeStr(xelement, "Name");
					text2 = text2.Replace("\\", "/");
					string persistentPath = PathUtils.GetPersistentPath(text2);
					if (File.Exists(persistentPath))
					{
						this.SetProcessText(string.Format(Global.GetLang("正在移除本地文件{0} "), persistentPath));
						File.Delete(persistentPath);
						MUDebug.Log<string>(new string[]
						{
							"Delete file:" + persistentPath
						});
					}
					XElement xelement3 = Global.GetXElement(Global.PersistentIndexXML, "File", "Name", text.ToString());
					if (xelement3 != null)
					{
						xelement3.Remove();
					}
				}
			}
		}
	}

	private void InitDeleteZIPItem()
	{
		if (this.NeedToDeleteZIPFileList != null && this.NeedToDeleteZIPFileList.Count > 0 && Global.PersistentZIPIndexXML != null)
		{
			MUDebug.Log<string>(new string[]
			{
				"InitDeleteZIPItem Count:" + this.NeedToDeleteZIPFileList.Count
			});
			XElement xelement = null;
			Dictionary<string, XElement> dictionary = new Dictionary<string, XElement>();
			List<XElement> xelementList = Global.GetXElementList(Global.PersistentZIPIndexXML, "File");
			foreach (XElement xelement2 in xelementList)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "Name");
				dictionary[xelementAttributeStr] = xelement2;
			}
			this.HintUser1.text = Global.GetLang("提示：正在清理冗余资源");
			foreach (string text in this.NeedToDeleteZIPFileList)
			{
				if (dictionary.TryGetValue(text, ref xelement))
				{
					string text2 = Global.GetXElementAttributeStr(xelement, "Name");
					text2 = text2.Replace("\\", "/");
					string persistentPath = PathUtils.GetPersistentPath(text2);
					if (File.Exists(persistentPath))
					{
						this.SetProcessText(string.Format(Global.GetLang("正在移除本地文件{0} "), persistentPath));
						File.Delete(persistentPath);
						MUDebug.Log<string>(new string[]
						{
							"Delete file:" + persistentPath
						});
					}
					XElement xelement3 = Global.GetXElement(Global.PersistentZIPIndexXML, "File", "Name", text.ToString());
					if (xelement3 != null)
					{
						xelement3.Remove();
					}
				}
			}
		}
	}

	private IEnumerator InitUpdate()
	{
		if (this.NextStep == null)
		{
			yield return null;
		}
		WWW www = null;
		this.WebErrMsg.Text = string.Empty;
		this.ProgressCenter.Percent = 1.0;
		this.ProgressCenter.ProgessText = string.Empty;
		this.progressText.Text = string.Empty;
		this.DownloadedSize = 0.0;
		this.SetProcessText(Global.GetLang("开始对比版本差异"));
		this.InitDeleteItem();
		Super.UpdateGameInstance.MapPic.ImageURL = "LoadGame/11.jpg";
		Super.UpdateGameInstance.MapPic.ForceShow();
		string persistentIndexXMLFile = PathUtils.GetPersistentPath("index.xml");
		if (this.LocalAppVerCode < this.RemoteAppVerCode && !File.Exists(PathUtils.GetPersistentPath(UpdateGame.UpdatePackageName)))
		{
			XElement newXmlItem = new XElement("File");
			newXmlItem.SetAttributeValue("Name", UpdateGame.UpdatePackageName);
			newXmlItem.SetAttributeValue("FileSize", 94371840.ToString());
			this.NeedToUpdateFileList.Add(newXmlItem);
		}
		string url = this.URL;
		this.ProgressCenter.Percent = 0.0;
		this.ProgressCenter.ProgessText = string.Empty;
		int totalFileIndex = 0;
		this.TotalFileCount = this.NeedToUpdateFileList.Count;
		this.NeedDownloadTotalSize = (double)this.GetTotalNeedDownloadSize();
		if (this.NeedDownloadTotalSize >= (double)this.DivideSize)
		{
			int count = (int)(this.NeedDownloadTotalSize / (double)this.UnitSize);
			if (this.NeedDownloadTotalSize % (double)this.UnitSize > (double)this.UnitSize * 0.5)
			{
				count++;
			}
			this.TotalFileCount = count;
			if (this.TotalFileCount == 0)
			{
				this.TotalFileCount = 1;
			}
			this.UnitSize = (int)(this.NeedDownloadTotalSize / (double)this.TotalFileCount);
		}
		base.InvokeRepeating("UpdateFileProgress", UpdateGame.RepeatRateForProcess, UpdateGame.RepeatRateForProcess);
		this.UpdateStep = 0;
		XElement rootIndex = Global.GetXElement(Global.PersistentIndexXML, "Files");
		string IndexPersistentPath = PathUtils.GetPersistentPath("index.xml");
		MUDebug.Log<string>(new string[]
		{
			"needToUpdateFileList.Count+" + this.NeedToUpdateFileList.Count
		});
		if (this.NeedToUpdateFileList.Count > 0)
		{
			this.HintUser1.text = Global.GetLang("提示：建议您在WIFI网络环境下载更新文件");
			if ((double)(QMQJJava.GetLeftAvailableMemery() * 1024L * 1024L) < this.NeedDownloadTotalSize)
			{
				string needSpace = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					(int)(this.NeedDownloadTotalSize / 1048576.0 + 1.0) + "MB"
				});
				string speceType = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					(!QMQJJava.CheckSDCard()) ? Global.GetLang("内部机身存储") : Global.GetLang("SD卡存储")
				});
				this.HintUser1.text = string.Format(Global.GetLang("本次更新至少需{0}的可用{1}空间"), needSpace, speceType);
			}
		}
		bool isAllUpdateOK = true;
		List<int> UpdateFailedList = new List<int>();
		for (int i = 0; i < this.NeedToUpdateFileList.Count; i++)
		{
			XElement fileXmlItem = this.NeedToUpdateFileList[i];
			string fileName = Global.GetXElementAttributeStr(fileXmlItem, "Name");
			string crc32 = Global.GetXElementAttributeStr(fileXmlItem, "CRC32");
			float fileSize = (float)Global.GetXElementAttributeInt(fileXmlItem, "FileSize");
			fileName = fileName.Replace("\\", "/");
			MUDebug.Log<string>(new string[]
			{
				"Download file=" + fileName
			});
			if (this.NeedDownloadTotalSize < (double)this.DivideSize)
			{
				MUDebug.Log<string>(new string[]
				{
					"YN_debug:InitUpdate_小于固定大小：DownloadedSize=" + this.DownloadedSize
				});
				this.SetProcessText(string.Format(Global.GetLang("正在下载更新文件({0}/{1})"), totalFileIndex, this.TotalFileCount));
			}
			string wwwPath = PathUtils.GetWWWPath(url + fileName);
			if (crc32 != null && crc32 != string.Empty)
			{
				wwwPath = wwwPath + "?v=" + crc32;
			}
			using (www = new WWW(wwwPath))
			{
				this.download = www;
				this.CurrentDownloadSize = fileSize;
				MUDebug.Log<string>(new string[]
				{
					www.url
				});
				yield return www;
				if (!string.IsNullOrEmpty(www.error))
				{
					UpdateFailedList.Add(i);
					MUDebug.Log<string>(new string[]
					{
						www.error
					});
					this.download = null;
					this.CurrentDownloadSize = 0f;
					www.Dispose();
					www = null;
				}
				else
				{
					rootIndex.Add(fileXmlItem);
					totalFileIndex++;
					Global.SaveBytesToFile(PathUtils.GetPersistentPath(fileName), www.bytes);
					this.DownloadedSize += (double)fileSize;
					XmlManager.SaveXElementToFile(IndexPersistentPath, Global.PersistentIndexXML);
					this.download = null;
					this.CurrentDownloadSize = 0f;
					www.Dispose();
					www = null;
				}
			}
		}
		if (UpdateFailedList.Count > 0)
		{
			MUDebug.Log<string>(new string[]
			{
				"再次尝试下载失败的资源"
			});
			for (int j = 0; j < UpdateFailedList.Count; j++)
			{
				XElement fileXmlItem2 = this.NeedToUpdateFileList[UpdateFailedList[j]];
				string fileName2 = Global.GetXElementAttributeStr(fileXmlItem2, "Name");
				string crc33 = Global.GetXElementAttributeStr(fileXmlItem2, "CRC32");
				float fileSize2 = (float)Global.GetXElementAttributeInt(fileXmlItem2, "FileSize");
				fileName2 = fileName2.Replace("\\", "/");
				MUDebug.Log<string>(new string[]
				{
					"Download file=" + fileName2
				});
				if (this.NeedDownloadTotalSize < (double)this.DivideSize)
				{
					MUDebug.Log<string>(new string[]
					{
						"YN_debug:InitUpdate_UpdateFailedList_小于固定大小：DownloadedSize=" + this.DownloadedSize
					});
					this.SetProcessText(string.Format(Global.GetLang("正在下载更新文件({0}/{1})"), totalFileIndex, this.TotalFileCount));
				}
				string wwwPath2 = PathUtils.GetWWWPath(url + fileName2);
				if (crc33 != null && crc33 != string.Empty)
				{
					wwwPath2 = wwwPath2 + "?v=" + crc33;
				}
				using (www = new WWW(wwwPath2))
				{
					this.download = www;
					this.CurrentDownloadSize = fileSize2;
					MUDebug.Log<string>(new string[]
					{
						www.url
					});
					yield return www;
					if (!string.IsNullOrEmpty(www.error))
					{
						if (fileName2.IndexOf(".png") == -1 && fileName2.IndexOf(".PNG") == -1 && fileName2.IndexOf(".JPG") == -1 && fileName2.IndexOf(".jpg") == -1)
						{
							XmlManager.SaveXElementToFile(PathUtils.GetPersistentPath("index.xml"), Global.PersistentIndexXML);
							GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接(更新),请检查您的网络是否正常！  ") + www.error, -1, -1, -1, -1, 0.7, default(Vector3), null, null);
							messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
							{
								int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
								Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
								Application.Quit();
								return true;
							};
							yield break;
						}
						isAllUpdateOK = false;
						MUDebug.Log<string>(new string[]
						{
							"isAllUpdateOK = false " + www.url
						});
						MUDebug.Log<string>(new string[]
						{
							www.error
						});
						this.download = null;
						this.CurrentDownloadSize = 0f;
						www.Dispose();
						www = null;
					}
					else
					{
						rootIndex.Add(fileXmlItem2);
						totalFileIndex++;
						Global.SaveBytesToFile(PathUtils.GetPersistentPath(fileName2), www.bytes);
						this.DownloadedSize += (double)fileSize2;
						XmlManager.SaveXElementToFile(IndexPersistentPath, Global.PersistentIndexXML);
						this.download = null;
						this.CurrentDownloadSize = 0f;
						www.Dispose();
						www = null;
					}
				}
			}
		}
		this.ProgressCenter.Percent = 1.0;
		if (this.LocalAppVerCode < this.RemoteAppVerCode)
		{
			this.ProgressCenter.Percent = 1.0;
			this.ProgressCenter.ProgessText = string.Empty;
			totalFileIndex = 0;
			this.TotalFileCount = 0;
			List<XElement> fileList = Global.GetXElementList(Global.IndexXML, "File");
			if (fileList != null && fileList.Count > 0)
			{
				this.HintUser1.text = Global.GetLang("提示：加载更新文件不会产生流量，请耐心等待。");
			}
			this.TotalFileCount = fileList.Count;
			this.UpdateStep = 1;
			for (int k = 0; k < fileList.Count; k++)
			{
				XElement fileXmlItem3 = fileList[k];
				string fileName3 = Global.GetXElementAttributeStr(fileXmlItem3, "Name");
				fileName3 = fileName3.Replace("\\", "/");
				MUDebug.Log<string>(new string[]
				{
					"Copy file=" + fileName3
				});
				totalFileIndex++;
				this.SetProcessText(string.Format(Global.GetLang("正在加载更新文件({0}/{1})"), totalFileIndex, this.TotalFileCount));
				if (!File.Exists(PathUtils.GetPersistentPath(fileName3)))
				{
					using (www = new WWW(PathUtils.SteamingAssetsPath_DontUseThis(fileName3)))
					{
						this.download = www;
						MUDebug.Log<string>(new string[]
						{
							www.url
						});
						yield return www;
						if (!string.IsNullOrEmpty(www.error))
						{
							MUDebug.Log<string>(new string[]
							{
								www.error
							});
							this.download = null;
							isAllUpdateOK = false;
							MUDebug.Log<string>(new string[]
							{
								"sAllUpdateOK = false;" + www.url
							});
						}
						else
						{
							Global.SaveBytesToFile(PathUtils.GetPersistentPath(fileName3), www.bytes);
							rootIndex.Add(fileXmlItem3);
							XmlManager.SaveXElementToFile(IndexPersistentPath, Global.PersistentIndexXML);
							this.download = null;
							www.Dispose();
						}
					}
				}
			}
		}
		if (this.RemoteVersionBytes != null && isAllUpdateOK)
		{
			MUDebug.Log<string>(new string[]
			{
				"Global.SaveBytesToFile  version.xml"
			});
			Global.SaveBytesToFile(PathUtils.GetPersistentPath("version.xml"), this.RemoteVersionBytes);
		}
		XmlManager.SaveXElementToFile(IndexPersistentPath, Global.PersistentIndexXML);
		base.CancelInvoke("UpdateFileProgress");
		MainGame._current.CancelCheckHasNetwork();
		if (this.LocalAppVerCode < this.RemoteAppVerCode)
		{
			string apkName = PathUtils.GetPersistentPath(UpdateGame.UpdatePackageName);
			if (File.Exists(apkName))
			{
				GChildWindow messageBoxWindow2 = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("游戏资源已经更新，请安装最新的游戏包后重新进入游戏！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow2.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow2.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow2);
					PlayerPrefs.SetInt("NoticeShowed", 0);
					QMQJJava.LaunchAPK(apkName);
					Application.Quit();
					return true;
				};
			}
			else
			{
				GChildWindow messageBoxWindow3 = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang(Global.GetLang("错误")), Global.GetLang("游戏更新失败，请重新下载完整包安装后，再运行！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow3.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow3.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow3);
					return true;
				};
			}
			yield break;
		}
		this.UpdateOver(isAllUpdateOK);
		if (this.NextStep != null)
		{
			this.NextStep(this, new NextStepEventArgs
			{
				StepType = 0
			});
		}
		yield break;
	}

	private IEnumerator InitZIPUpdate()
	{
		if (this.NextStep == null)
		{
			yield return null;
		}
		WWW www = null;
		this.WebErrMsg.Text = string.Empty;
		this.ProgressCenter.Percent = 1.0;
		this.ProgressCenter.ProgessText = string.Empty;
		this.progressText.Text = string.Empty;
		this.DownloadedSize = 0.0;
		this.SetProcessText(Global.GetLang("开始对比版本差异"));
		this.InitDeleteZIPItem();
		Super.UpdateGameInstance.MapPic.ImageURL = "LoadGame/11.jpg";
		Super.UpdateGameInstance.MapPic.ForceShow();
		string url = this.URL + "/ZIP/";
		this.ProgressCenter.Percent = 0.0;
		this.ProgressCenter.ProgessText = string.Empty;
		int totalFileIndex = 0;
		this.TotalFileCount = this.NeedToUpdateZIPFileList.Count;
		this.NeedDownloadTotalSize = (double)this.GetTotalNeedDownloadZIPSize();
		MUDebug.Log<string>(new string[]
		{
			"NeedDownloadTotalSize:" + this.NeedDownloadTotalSize
		});
		if (this.NeedDownloadTotalSize >= (double)this.DivideSize)
		{
			int count = (int)(this.NeedDownloadTotalSize / (double)this.UnitSize);
			if (this.NeedDownloadTotalSize % (double)this.UnitSize > (double)this.UnitSize * 0.5)
			{
				count++;
			}
			this.TotalFileCount = count;
			if (this.TotalFileCount == 0)
			{
				this.TotalFileCount = 1;
			}
			this.UnitSize = (int)(this.NeedDownloadTotalSize / (double)this.TotalFileCount);
		}
		base.InvokeRepeating("UpdateFileProgress", UpdateGame.RepeatRateForProcess, UpdateGame.RepeatRateForProcess);
		this.UpdateStep = 0;
		XElement rootIndex = Global.GetXElement(Global.PersistentZIPIndexXML, "Files");
		string IndexPersistentPath = PathUtils.GetPersistentPath("ZIPIndex.xml");
		MUDebug.Log<string>(new string[]
		{
			"needToUpdateFileList.Count+" + this.NeedToUpdateZIPFileList.Count
		});
		if (this.NeedToUpdateZIPFileList.Count > 0)
		{
			this.HintUser1.text = Global.GetLang("提示：建议您在WIFI网络环境下载更新文件");
			if ((double)(QMQJJava.GetLeftAvailableMemery() * 1024L * 1024L) < this.NeedDownloadTotalSize)
			{
				string needSpace = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					(int)(this.NeedDownloadTotalSize / 1048576.0 + 1.0) + "MB"
				});
				string speceType = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					(!QMQJJava.CheckSDCard()) ? Global.GetLang("内部机身存储") : Global.GetLang("SD卡存储")
				});
				this.HintUser1.text = string.Format(Global.GetLang("本次更新至少需{0}的可用{1}空间"), needSpace, speceType);
			}
		}
		bool isAllUpdateOK = true;
		List<int> UpdateFailedList = new List<int>();
		for (int i = 0; i < this.NeedToUpdateZIPFileList.Count; i++)
		{
			XElement fileXmlItem = this.NeedToUpdateZIPFileList[i];
			string fileName = Global.GetXElementAttributeStr(fileXmlItem, "Name");
			string crc32 = Global.GetXElementAttributeStr(fileXmlItem, "CRC32");
			float fileSize = (float)Global.GetXElementAttributeInt(fileXmlItem, "FileSize");
			fileName = fileName.Replace("\\", "/");
			MUDebug.Log<string>(new string[]
			{
				"Download file=" + fileName
			});
			if (this.NeedDownloadTotalSize < (double)this.DivideSize)
			{
				MUDebug.Log<string>(new string[]
				{
					"YN_debug:InitZIPUpdate_小于固定大小：DownloadedSize=" + this.DownloadedSize
				});
				this.SetProcessText(string.Format(Global.GetLang("正在下载更新文件({0}/{1})"), totalFileIndex, this.TotalFileCount));
			}
			string wwwPath = PathUtils.GetWWWPath(url + fileName);
			if (crc32 != null && crc32 != string.Empty)
			{
				wwwPath = wwwPath + "?v=" + crc32;
			}
			using (www = new WWW(wwwPath))
			{
				this.download = www;
				this.CurrentDownloadSize = fileSize;
				MUDebug.Log<string>(new string[]
				{
					www.url
				});
				yield return www;
				if (!string.IsNullOrEmpty(www.error))
				{
					UpdateFailedList.Add(i);
					MUDebug.Log<string>(new string[]
					{
						www.error
					});
					this.download = null;
					this.CurrentDownloadSize = 0f;
					www.Dispose();
					www = null;
				}
				else
				{
					rootIndex.Add(fileXmlItem);
					totalFileIndex++;
					Global.SaveBytesToFile(PathUtils.GetPersistentPath(fileName), www.bytes);
					MUDebug.Log<string>(new string[]
					{
						"Saved zip:" + PathUtils.GetPersistentPath(fileName)
					});
					this.DownloadedSize += (double)fileSize;
					this.UnZipAndDeleteZIP(fileName);
					XmlManager.SaveXElementToFile(IndexPersistentPath, Global.PersistentZIPIndexXML);
					this.download = null;
					this.CurrentDownloadSize = 0f;
					www.Dispose();
					www = null;
				}
			}
		}
		if (UpdateFailedList.Count > 0)
		{
			MUDebug.Log<string>(new string[]
			{
				"再次尝试下载失败的资源"
			});
			for (int j = 0; j < UpdateFailedList.Count; j++)
			{
				XElement fileXmlItem2 = this.NeedToUpdateZIPFileList[UpdateFailedList[j]];
				string fileName2 = Global.GetXElementAttributeStr(fileXmlItem2, "Name");
				string crc33 = Global.GetXElementAttributeStr(fileXmlItem2, "CRC32");
				float fileSize2 = (float)Global.GetXElementAttributeInt(fileXmlItem2, "FileSize");
				fileName2 = fileName2.Replace("\\", "/");
				MUDebug.Log<string>(new string[]
				{
					"Download file=" + fileName2
				});
				if (this.NeedDownloadTotalSize < (double)this.DivideSize)
				{
					MUDebug.Log<string>(new string[]
					{
						"YN_debug:InitZIPUpdate_UpdateFailedList_小于固定大小：DownloadedSize=" + this.DownloadedSize
					});
					this.SetProcessText(string.Format(Global.GetLang("正在下载更新文件({0}/{1})"), totalFileIndex, this.TotalFileCount));
				}
				string wwwPath2 = PathUtils.GetWWWPath(url + fileName2);
				if (crc33 != null && crc33 != string.Empty)
				{
					wwwPath2 = wwwPath2 + "?v=" + crc33;
				}
				using (www = new WWW(wwwPath2))
				{
					this.download = www;
					this.CurrentDownloadSize = fileSize2;
					MUDebug.Log<string>(new string[]
					{
						www.url
					});
					yield return www;
					if (!string.IsNullOrEmpty(www.error))
					{
						XmlManager.SaveXElementToFile(PathUtils.GetPersistentPath("ZIPIndex.xml"), Global.PersistentZIPIndexXML);
						GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接(更新2),请检查您的网络是否正常！") + www.error, -1, -1, -1, -1, 0.7, default(Vector3), null, null);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
							Application.Quit();
							return true;
						};
						yield break;
					}
					rootIndex.Add(fileXmlItem2);
					totalFileIndex++;
					Global.SaveBytesToFile(PathUtils.GetPersistentPath(fileName2), www.bytes);
					this.DownloadedSize += (double)fileSize2;
					this.UnZipAndDeleteZIP(fileName2);
					XmlManager.SaveXElementToFile(IndexPersistentPath, Global.PersistentZIPIndexXML);
					this.download = null;
					this.CurrentDownloadSize = 0f;
					www.Dispose();
					www = null;
				}
			}
		}
		XmlManager.SaveXElementToFile(IndexPersistentPath, Global.PersistentZIPIndexXML);
		base.CancelInvoke("UpdateFileProgress");
		this.UpdateZIPOver(isAllUpdateOK);
		Super.ShowCheckingUpdateGame(MainGame._current.Stage);
		base.StartCoroutine<bool>(UpdateGame.CheckFileIndex());
		yield break;
	}

	private void UnZipAndDeleteZIP(string fileName)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		fileName = fileName.Replace("\\", "/");
		string persistentPath = PathUtils.GetPersistentPath(fileName);
		MUDebug.Log<string>(new string[]
		{
			"Unzip:" + persistentPath
		});
		string[] array = new string[]
		{
			persistentPath,
			PathUtils.GetPersistentPath(string.Empty)
		};
		UnZipFloClass unZipFloClass = new UnZipFloClass();
		unZipFloClass.unZipFile(array[0], array[1]);
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"benginUnzipTime:",
				realtimeSinceStartup,
				"___End UnZip:",
				Time.realtimeSinceStartup
			})
		});
		if (File.Exists(persistentPath))
		{
			File.Delete(persistentPath);
			MUDebug.Log<string>(new string[]
			{
				"Delete file:" + persistentPath
			});
		}
	}

	private void UpdateOver(bool isAllUpdateOK)
	{
		if (isAllUpdateOK)
		{
			if (Global.IsMiniPackage)
			{
				Global.IsMiniPackUpdated = true;
				Global.IsFullPackUpdated = false;
				LocalStorage.SetString("IsMiniPackUpdated", "true");
				LocalStorage.SetString("IsFullPackUpdatedApp", "false");
				LocalStorage.SetString("IsFullPackUpdatedRes", "false");
			}
			else
			{
				Global.IsMiniPackUpdated = false;
				Global.IsFullPackUpdated = true;
				LocalStorage.SetString("IsMiniPackUpdated", "false");
				LocalStorage.SetString("IsFullPackUpdatedApp", Global.ReadXmlConfigInt(Global.VersionXml, "Application", "VerCode").ToString());
				LocalStorage.SetString("IsFullPackUpdatedRes", Global.ReadXmlConfigInt(Global.VersionXml, "Resource", "VerCode").ToString());
			}
		}
		else
		{
			LocalStorage.SetString("IsMiniPackUpdated", "false");
			LocalStorage.SetString("IsFullPackUpdatedApp", "false");
			LocalStorage.SetString("IsFullPackUpdatedRes", "false");
		}
		this.NeedToUpdateFileList = null;
		this.NeedToDeleteFileList = null;
		Global.IndexXML = null;
		Global.PersistentIndexXML = null;
		Global.PersistentVersionXML = null;
		Global.NetIndexXML = null;
		AssetBundleManager.RemoveAssetBundle("index.unity3d");
		XmlManager.RemoveXElement("index");
	}

	private void UpdateZIPOver(bool isAllZIPUpdateOK)
	{
		if (isAllZIPUpdateOK)
		{
			Global.IsMiniPackZipUpdated = true;
			LocalStorage.SetString("IsMiniPackZipUpdated", "true");
		}
		else
		{
			LocalStorage.SetString("IsMiniPackZipUpdated", "false");
		}
		this.NeedToUpdateZIPFileList = null;
		this.NeedToDeleteZIPFileList = null;
		Global.PersistentZIPIndexXML = null;
		Global.NetZIPIndexXML = null;
	}

	public void UpdateFileProgress()
	{
		if (this.UpdateStep == 0)
		{
			if (this.NeedDownloadTotalSize < (double)this.DivideSize)
			{
				float num = 0f;
				if (this.download != null && !this.download.isDone)
				{
					num = this.download.progress;
				}
				this.ProgressCenter.Percent = (double)num;
				this.ProgressCenter.ProgessText = ((int)(num * 100f)).ToString() + "%";
			}
			else
			{
				float num2 = 0f;
				double num3 = this.DownloadedSize;
				if (this.download != null && !this.download.isDone)
				{
					num3 += (double)(this.CurrentDownloadSize * this.download.progress);
				}
				double num4 = num3 % (double)this.UnitSize;
				num2 = (float)(num4 / (double)this.UnitSize);
				int num5 = 0;
				if (this.IsLastDownLoadFile)
				{
					num5 = (int)Math.Round(num3 / (double)this.UnitSize);
					this.IsLastDownLoadFile = false;
				}
				else
				{
					num5 = (int)(num3 / (double)this.UnitSize);
				}
				if (num4 > 1.0)
				{
					num5++;
				}
				if (num5 >= this.TotalFileCount)
				{
					num5 = this.TotalFileCount;
				}
				if (num5 >= 76)
				{
					MUDebug.Log<string>(new string[]
					{
						string.Concat(new object[]
						{
							"验证资源加载最后出现数值问题tempFileIndex = ",
							num5,
							"::tempSize = ",
							num3,
							"::UnitSize = ",
							this.UnitSize
						})
					});
				}
				try
				{
					double num6 = Math.Round(num3 / 1024.0 / 1024.0, 2);
					this.HasDownloadValue = Math.Round(num6, 2).ToString("0.0");
					double num7 = Math.Round(this.DownloadedSize / 1024.0 / 1024.0, 2);
					MUDebug.Log<string>(new string[]
					{
						string.Format("YN_debug--UpdateFileProgress:tempHasDownSize={0};HasDownloadValue={1};tempDownloadedSize={2}", num6, this.HasDownloadValue, num7)
					});
				}
				catch
				{
					MUDebug.LogError<string>(new string[]
					{
						"YN_debug--UpdateFileProgress_HasDownloadValue has a error!"
					});
				}
				this.SetProcessText(string.Format(Global.GetLang("正在下载更新文件({0}/{1}),资源大小({2}MB/{3}MB)"), new object[]
				{
					num5,
					this.TotalFileCount,
					this.HasDownloadValue,
					this.NeedDownSizeValue
				}));
				this.ProgressCenter.Percent = (double)num2;
				this.ProgressCenter.ProgessText = ((int)(num2 * 100f)).ToString() + "%";
				if (num5 == this.TotalFileCount)
				{
					this.IsLastDownLoadFile = true;
				}
			}
		}
		else
		{
			this.ProgressCenter.Percent = 1.0;
			this.ProgressCenter.ProgessText = string.Empty;
		}
	}

	private float GetTotalNeedDownloadSize()
	{
		float num = 0f;
		int count = this.NeedToUpdateFileList.Count;
		for (int i = 0; i < count; i++)
		{
			XElement xelement = this.NeedToUpdateFileList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "FileSize");
			if (xelementAttributeInt > 0)
			{
				num += (float)xelementAttributeInt;
			}
		}
		return num;
	}

	private float GetTotalNeedDownloadZIPSize()
	{
		float num = 0f;
		int count = this.NeedToUpdateZIPFileList.Count;
		for (int i = 0; i < count; i++)
		{
			XElement xelement = this.NeedToUpdateZIPFileList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "FileSize");
			if (xelementAttributeInt > 0)
			{
				num += (float)xelementAttributeInt;
			}
		}
		return num;
	}

	private static void StopTimer()
	{
		if (UpdateGame.Timer != null)
		{
			UpdateGame.Timer.Tick = null;
			UpdateGame.Timer.Stop();
			UpdateGame.Timer = null;
		}
		UpdateGame.TotalWaitCount = 0;
	}

	private static void StartTimer()
	{
		if (UpdateGame.Timer != null)
		{
			UpdateGame.Timer.Tick = null;
			UpdateGame.Timer.Stop();
			UpdateGame.Timer = null;
		}
		UpdateGame.Timer = new DispatcherTimer("UpdateGame_Timer");
		UpdateGame.Timer.Interval = TimeSpan.FromSeconds(1.0);
		UpdateGame.Timer.Tick = new DispatcherTimerEventHandler(UpdateGame.Timer_Tick);
		UpdateGame.Timer.Start();
	}

	private static void Timer_Tick(object sender, object e)
	{
		UpdateGame.TotalWaitCount++;
		if (UpdateGame.TotalWaitCount > 30)
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("勇士,您的网络已断开连接(超时),请检查您的网络是否正常！"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				Application.Quit();
				return true;
			};
			UpdateGame.StopTimer();
		}
	}

	public GImgProgressBar ProgressCenter;

	public GTextBlockOutLine progressText;

	public GTextBlockOutLine WebErrMsg;

	public GTextBlockOutLine HintUser1;

	public ShowNetImage MapPic;

	private static byte[] remoteVersionBytes;

	private static float starttime;

	public List<string> NeedToDeleteFileList;

	public List<XElement> NeedToUpdateFileList;

	public List<string> NeedToDeleteZIPFileList;

	public List<XElement> NeedToUpdateZIPFileList;

	private double DownloadedSize;

	private double NeedDownloadTotalSize;

	private int TotalFileCount;

	private static float RepeatRateForProcess = 0.1f;

	private static string UpdatePackageName = "ClientUpdate.apk";

	public NextStepEventHandler NextStep;

	public static NextStepEventHandler UpdateNextStep;

	private string NeedDownSizeValue;

	private string HasDownloadValue;

	private WWW download;

	private float CurrentDownloadSize;

	private int UpdateStep;

	private int DivideSize;

	private int UnitSize = 10485760;

	private bool IsLastDownLoadFile;

	private static DispatcherTimer Timer;

	private static int TotalWaitCount;
}
