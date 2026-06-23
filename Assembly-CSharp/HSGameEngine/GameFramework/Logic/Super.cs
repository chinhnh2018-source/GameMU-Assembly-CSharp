using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Tools;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

namespace HSGameEngine.GameFramework.Logic
{
	public static class Super
	{
		public static void ActiveModalLayer(bool active)
		{
			Global.DisableInput = active;
			if (null != Super.ModalLayer)
			{
				Super.ModalLayer.SetActive(active);
			}
		}

		public static void ActiveDialogLayer(bool active)
		{
			if (null != Super.DialogLayer)
			{
				Super.DialogLayer.SetActive(active);
			}
		}

		public static void ShowNetWaiting(string hint = null)
		{
			if (null == Super.NetWaiting)
			{
				return;
			}
			if (!Super.NetWaiting.activeSelf)
			{
				Super.NetWaiting.SetActive(true);
			}
		}

		public static void HideNetWaiting()
		{
			if (null == Super.NetWaiting)
			{
				return;
			}
			if (Super.NetWaiting.activeSelf)
			{
				Super.NetWaiting.SetActive(false);
			}
		}

		public static GChildWindow ShowMessageBoxByPosition(Canvas root, int boxType, string caption, string message, [Optional] Vector3 MessagePosition, [Optional] Vector3 OkBtnPosition, [Optional] Vector3 CancelBtnPosition, string OkButtonText = null, string CancelButtonText = null, int labelMaxWidth = 0, [Optional] Vector3 pos, MouseLeftButtonUpEventHandler RegOkCallBack = null)
		{
			GChildWindow messageBoxWindow = U3DUtils.NEW<GChildWindow>();
			MyMessageBoxPart messageBoxPart = U3DUtils.NEW<MyMessageBoxPart>();
			Super.InitChildWindow(messageBoxWindow, "ShowMessageBoxByPosition");
			messageBoxWindow.ModalType = ChildWindowModalType.Translucent;
			messageBoxPart.BoxType = boxType;
			messageBoxPart.HintTitle = caption;
			messageBoxPart.HintText = message;
			if (labelMaxWidth != 0)
			{
				messageBoxPart.HintText_Label.MaxWidth = (double)labelMaxWidth;
			}
			if (boxType == 0)
			{
				if (OkBtnPosition != Vector3.one)
				{
					messageBoxPart.OkBtn.transform.localPosition = OkBtnPosition;
				}
				if (MessagePosition != Vector3.one)
				{
					messageBoxPart.HintText_Label.transform.localPosition = MessagePosition;
				}
			}
			else if (boxType == 1)
			{
				if (OkBtnPosition != Vector3.one)
				{
					messageBoxPart.OkBtn.transform.localPosition = OkBtnPosition;
				}
				if (CancelBtnPosition != Vector3.one)
				{
					messageBoxPart.CancelBtn.transform.localPosition = CancelBtnPosition;
				}
				if (MessagePosition != Vector3.one)
				{
					messageBoxPart.HintText_Label.transform.localPosition = MessagePosition;
				}
			}
			messageBoxPart.ButtonClick = delegate(object s, EventArgs e)
			{
				int myMessageBoxPartReturn = messageBoxPart.MyMessageBoxPartReturn;
				messageBoxWindow.NotifyClose(myMessageBoxPartReturn);
				Super.m_MessageBoxPart = null;
				Super.CloseChildWindow(root, messageBoxWindow);
			};
			if (RegOkCallBack != null)
			{
				messageBoxPart.OkBtn.MouseLeftButtonUp = RegOkCallBack;
				GButton okBtn = messageBoxPart.OkBtn;
				okBtn.MouseLeftButtonUp = (MouseLeftButtonUpEventHandler)Delegate.Combine(okBtn.MouseLeftButtonUp, delegate(object o, MouseEvent e)
				{
					Super.CloseChildWindow(root, messageBoxWindow);
				});
			}
			if (OkButtonText != null)
			{
				messageBoxPart.OkBtn.Text = OkButtonText;
			}
			if (CancelButtonText != null)
			{
				messageBoxPart.CancelBtn.Text = CancelButtonText;
			}
			messageBoxWindow.SetContent(messageBoxWindow.BodyPresenter, messageBoxPart, 9.0, 0.0, true);
			root.Children.Add(messageBoxWindow);
			messageBoxWindow.X = (double)pos.x;
			messageBoxWindow.Y = (double)pos.y;
			return messageBoxWindow;
		}

		public static GChildWindow ShowMessageBox(Canvas root, int boxType, string caption, string message, int left = -1, int top = -1, int width = -1, int height = -1, double opacity = 0.7, [Optional] Vector3 pos, MouseLeftButtonUpEventHandler RegOkCallBack = null, string OkButtonText = null)
		{
			GChildWindow messageBoxWindow = U3DUtils.NEW<GChildWindow>();
			MyMessageBoxPart messageBoxPart = U3DUtils.NEW<MyMessageBoxPart>();
			Super.InitChildWindow(messageBoxWindow, "messageBoxWindow");
			messageBoxWindow.ModalType = ChildWindowModalType.Translucent;
			messageBoxPart.BoxType = boxType;
			messageBoxPart.HintTitle = caption;
			messageBoxPart.HintText = message;
			messageBoxPart.ButtonClick = delegate(object s, EventArgs e)
			{
				int myMessageBoxPartReturn = messageBoxPart.MyMessageBoxPartReturn;
				messageBoxWindow.NotifyClose(myMessageBoxPartReturn);
				Super.m_MessageBoxPart = null;
				Super.CloseChildWindow(root, messageBoxWindow);
			};
			if (RegOkCallBack != null)
			{
				messageBoxPart.OkBtn.MouseLeftButtonUp = RegOkCallBack;
				GButton okBtn = messageBoxPart.OkBtn;
				okBtn.MouseLeftButtonUp = (MouseLeftButtonUpEventHandler)Delegate.Combine(okBtn.MouseLeftButtonUp, delegate(object o, MouseEvent e)
				{
					Super.CloseChildWindow(root, messageBoxWindow);
				});
			}
			if (OkButtonText != null)
			{
				messageBoxPart.OkBtn.Text = OkButtonText;
			}
			messageBoxWindow.SetContent(messageBoxWindow.BodyPresenter, messageBoxPart, 9.0, 0.0, true);
			root.Children.Add(messageBoxWindow);
			messageBoxWindow.X = (double)pos.x;
			messageBoxWindow.Y = (double)pos.y;
			return messageBoxWindow;
		}

		public static GChildWindow ZuanShiShowMessageBox(string caption, string message, int boxType, DPSelectedItemEventHandler handler, MessBoxIsHintTypes messBoxIsHintTypes = MessBoxIsHintTypes.None, float ZhuanShiIconXTrans = 0f, string DaiBi = "", int zuanShi = 0)
		{
			GChildWindow messageBoxWindow = U3DUtils.NEW<GChildWindow>();
			ZuanShiMessageBox messageBoxPart = U3DUtils.NEW<ZuanShiMessageBox>();
			Super.InitChildWindow(messageBoxWindow, "messageBoxWindow");
			messageBoxWindow.ModalType = ChildWindowModalType.Translucent;
			messageBoxPart.BoxType = boxType;
			messageBoxPart.HintTitle = caption;
			messageBoxPart.HintText = message;
			if (boxType == 2 && messBoxIsHintTypes != MessBoxIsHintTypes.None)
			{
				bool check = Super.MessageBoxIsHint[(int)messBoxIsHintTypes] == 1;
				messageBoxPart.CheckBox.Check = check;
			}
			if (messBoxIsHintTypes == MessBoxIsHintTypes.JjingLingSkillAwarkHint)
			{
				messageBoxPart.ZhuanShiPosMove = true;
			}
			else if (messBoxIsHintTypes == MessBoxIsHintTypes.JjingLingYaoSai)
			{
				messageBoxPart.Pivot = 3;
			}
			if (ZhuanShiIconXTrans != 0f)
			{
				messageBoxPart.ZhuanShiIconTransX = ZhuanShiIconXTrans;
			}
			messageBoxPart.zuanShi = zuanShi;
			if (!string.IsNullOrEmpty(DaiBi))
			{
				messageBoxPart.DaiBi = DaiBi;
			}
			messageBoxPart.ButtonClick = delegate(object s, EventArgs e)
			{
				if (boxType == 2)
				{
					Super.SetArrayElement(Super.MessageBoxIsHint, (int)messBoxIsHintTypes, (!messageBoxPart.CheckBox.Check) ? 0 : 1);
				}
				int myMessageBoxPartReturn = messageBoxPart.MyMessageBoxPartReturn;
				messageBoxWindow.NotifyClose(myMessageBoxPartReturn);
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, messageBoxWindow);
				if (handler != null)
				{
					handler(s, new DPSelectedItemEventArgs
					{
						ID = myMessageBoxPartReturn
					});
				}
			};
			messageBoxWindow.SetContent(messageBoxWindow.BodyPresenter, messageBoxPart, 9.0, 0.0, true);
			Super.GData.GlobalPlayZone.Children.Add(messageBoxWindow);
			return messageBoxWindow;
		}

		public static GChildWindow ShowMessageBox(string caption, string message, int boxType, DPSelectedItemEventHandler handler, MessBoxIsHintTypes messBoxIsHintTypes = MessBoxIsHintTypes.None)
		{
			GChildWindow messageBoxWindow = U3DUtils.NEW<GChildWindow>();
			MyMessageBoxPart messageBoxPart = U3DUtils.NEW<MyMessageBoxPart>();
			Super.InitChildWindow(messageBoxWindow, "messageBoxWindow");
			messageBoxWindow.ModalType = ChildWindowModalType.Translucent;
			messageBoxPart.BoxType = boxType;
			messageBoxPart.HintTitle = caption;
			messageBoxPart.HintText = message;
			if (boxType == 2 && messBoxIsHintTypes != MessBoxIsHintTypes.None)
			{
				bool check = Super.MessageBoxIsHint[(int)messBoxIsHintTypes] == 1;
				messageBoxPart.CheckBox.Check = check;
			}
			messageBoxPart.ButtonClick = delegate(object s, EventArgs e)
			{
				int myMessageBoxPartReturn = messageBoxPart.MyMessageBoxPartReturn;
				if (boxType == 2 && myMessageBoxPartReturn == 0)
				{
					Super.SetArrayElement(Super.MessageBoxIsHint, (int)messBoxIsHintTypes, (!messageBoxPart.CheckBox.Check) ? 0 : 1);
				}
				messageBoxWindow.NotifyClose(myMessageBoxPartReturn);
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, messageBoxWindow);
				if (handler != null)
				{
					handler(s, new DPSelectedItemEventArgs
					{
						ID = myMessageBoxPartReturn
					});
				}
			};
			messageBoxWindow.SetContent(messageBoxWindow.BodyPresenter, messageBoxPart, 9.0, 0.0, true);
			Super.GData.GlobalPlayZone.Children.Add(messageBoxWindow);
			return messageBoxWindow;
		}

		public static GChildWindow ShowMessageBoxEx(string caption, string message, DPSelectedItemEventHandler handler, params string[] buttons)
		{
			GChildWindow messageBoxWindow = U3DUtils.NEW<GChildWindow>();
			Super.InitChildWindow(messageBoxWindow, "MessageBoxExWindow");
			Super.GData.GlobalPlayZone.Children.Add(messageBoxWindow);
			MyMessageBoxExPart messageBoxPart = U3DUtils.NEW<MyMessageBoxExPart>();
			Super.m_MessageBoxPart = messageBoxPart;
			messageBoxWindow.SetContent(messageBoxWindow.BodyPresenter, messageBoxPart, 9.0, 0.0, true);
			messageBoxWindow.ModalType = ChildWindowModalType.Translucent;
			messageBoxPart.InitPartData(caption, message, buttons);
			messageBoxPart.ButtonClick = delegate(object s, EventArgs e)
			{
				int myMessageBoxPartReturn = messageBoxPart.MyMessageBoxPartReturn;
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, messageBoxWindow);
				if (handler != null)
				{
					handler(s, new DPSelectedItemEventArgs
					{
						ID = myMessageBoxPartReturn
					});
				}
				Super.m_MessageBoxPart = null;
			};
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int myMessageBoxPartReturn = messageBoxPart.MyMessageBoxPartReturn;
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, messageBoxWindow);
				if (handler != null)
				{
					handler(s1, new DPSelectedItemEventArgs
					{
						ID = myMessageBoxPartReturn
					});
				}
				Super.m_MessageBoxPart = null;
				return true;
			};
			return messageBoxWindow;
		}

		public static GChildWindow ShowMessageBoxEx(string caption, string message, DPSelectedItemEventHandler handler, string[] buttons, bool ShowClose)
		{
			GChildWindow messageBoxWindow = U3DUtils.NEW<GChildWindow>();
			Super.InitChildWindow(messageBoxWindow, "MessageBoxExWindow");
			Super.GData.GlobalPlayZone.Children.Add(messageBoxWindow);
			MyMessageBoxExPart messageBoxPart = U3DUtils.NEW<MyMessageBoxExPart>();
			Super.m_MessageBoxPart = messageBoxPart;
			messageBoxWindow.SetContent(messageBoxWindow.BodyPresenter, messageBoxPart, 9.0, 0.0, true);
			messageBoxWindow.ModalType = ChildWindowModalType.Translucent;
			messageBoxPart.InitPartData(caption, message, buttons);
			messageBoxPart.ButtonClick = delegate(object s, EventArgs e)
			{
				int myMessageBoxPartReturn = messageBoxPart.MyMessageBoxPartReturn;
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, messageBoxWindow);
				if (handler != null)
				{
					handler(s, new DPSelectedItemEventArgs
					{
						ID = myMessageBoxPartReturn
					});
				}
				Super.m_MessageBoxPart = null;
			};
			messageBoxPart.SetCloseBtnEnable(ShowClose);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int myMessageBoxPartReturn = messageBoxPart.MyMessageBoxPartReturn;
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, messageBoxWindow);
				handler(s1, new DPSelectedItemEventArgs
				{
					ID = myMessageBoxPartReturn
				});
				Super.m_MessageBoxPart = null;
				return true;
			};
			return messageBoxWindow;
		}

		public static GChildWindow ShowMessageBoxGUI(string caption, string message, DPSelectedItemEventHandler handler, params string[] buttons)
		{
			GChildWindow messageBoxWindow = U3DUtils.NEW<GChildWindow>();
			messageBoxWindow.ModalType = ChildWindowModalType.TranslucentGUI;
			Super.InitChildWindow(messageBoxWindow, "MessageBoxExWindow");
			Super.GData.GlobalPlayZone.Children.Add(messageBoxWindow);
			MyMessageBoxExPart messageBoxPart = U3DUtils.NEW<MyMessageBoxExPart>();
			Super.m_MessageBoxPart = messageBoxPart;
			messageBoxWindow.SetContent(messageBoxWindow.BodyPresenter, messageBoxPart, 9.0, 0.0, true);
			messageBoxPart.InitPartData(caption, message, buttons);
			messageBoxPart.ButtonClick = delegate(object s, EventArgs e)
			{
				int myMessageBoxPartReturn = messageBoxPart.MyMessageBoxPartReturn;
				messageBoxWindow.NotifyClose(myMessageBoxPartReturn);
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, messageBoxWindow);
				handler(s, new DPSelectedItemEventArgs
				{
					ID = myMessageBoxPartReturn
				});
				Super.m_MessageBoxPart = null;
			};
			if (messageBoxWindow.ChildWindowClose != null)
			{
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					handler(s1, new DPSelectedItemEventArgs
					{
						ID = -1000
					});
					return true;
				};
			}
			return messageBoxWindow;
		}

		public static GChildWindow ShowMessageBoxExt(string caption, string message, float delay, DPSelectedItemEventHandler handler, params string[] buttons)
		{
			GChildWindow messageBoxWindow = U3DUtils.NEW<GChildWindow>();
			Super.InitChildWindow(messageBoxWindow, "MessageBoxExWindow");
			Super.GData.GlobalPlayZone.Children.Add(messageBoxWindow);
			MessageBoxExtPart messageBoxtPart = U3DUtils.NEW<MessageBoxExtPart>();
			messageBoxWindow.SetContent(messageBoxWindow.BodyPresenter, messageBoxtPart, 9.0, 0.0, true);
			messageBoxWindow.ModalType = ChildWindowModalType.Translucent;
			messageBoxtPart.InitPartData(caption, message, delay, buttons);
			messageBoxtPart.ButtonClick = delegate(object s, EventArgs e)
			{
				int myMessageBoxPartReturn = messageBoxtPart.MyMessageBoxPartReturn;
				messageBoxWindow.NotifyClose(myMessageBoxPartReturn);
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, messageBoxWindow);
				handler(s, new DPSelectedItemEventArgs
				{
					ID = myMessageBoxPartReturn
				});
			};
			return messageBoxWindow;
		}

		public static GChildWindow ShowMessageBox2(Canvas root, int boxType, string caption, string message, int rootWidth, int rootHeight)
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(root, 0, caption, message, (rootWidth - 300) / 2, (rootHeight - 163) / 2, rootWidth, rootHeight, 0.01, default(Vector3), null, null);
			messageBoxWindow.ModalType = ChildWindowModalType.Translucent;
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				Super.CloseMessageBox(root, messageBoxWindow);
				return true;
			};
			return messageBoxWindow;
		}

		public static void CloseMessageBox(Canvas root, GChildWindow messageBoxWindow)
		{
			Object.Destroy(messageBoxWindow.gameObject);
		}

		public static void ShowMessageBoxEx(Canvas root, int boxType, string caption, string message, int left = -1, int top = -1, int width = -1, int height = -1, bool refreshPage = false)
		{
			Super.ShowMessageBoxEx(root, boxType, caption, message, null, null, -1, -1, -1, -1, false);
		}

		public static void ShowMessageBoxEx(Canvas root, int boxType, string caption, string message, MouseLeftButtonUpEventHandler RegOkCallBack, string OkButtonText, int left = -1, int top = -1, int width = -1, int height = -1, bool refreshPage = false)
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(root, boxType, caption, message, left, top, width, height, 0.01, default(Vector3), RegOkCallBack, OkButtonText);
			messageBoxWindow.ModalType = ChildWindowModalType.TransBak;
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				Super.CloseMessageBox(root, messageBoxWindow);
				if (refreshPage)
				{
				}
				return true;
			};
		}

		public static void SetArrayElement(int[] arr, int index, int value)
		{
			if (arr == null)
			{
				return;
			}
			if (index < 0 || index >= arr.Length)
			{
				return;
			}
			arr[index] = value;
		}

		public static GChildWindow ShowLoginYiChang(Canvas root, string _comfirmIconText, string _message, int _second = 0, bool _fengHaoBl = false, [Optional] Vector3 pos)
		{
			GChildWindow messageBoxWindow = U3DUtils.NEW<GChildWindow>();
			ZhangHaoJingGao messageBoxPart = U3DUtils.NEW<ZhangHaoJingGao>();
			Super.InitChildWindow(messageBoxWindow, "messageBoxWindow");
			messageBoxWindow.ModalType = ChildWindowModalType.Translucent;
			messageBoxPart.HintText = _message;
			messageBoxPart.HintComfirmIconText = _comfirmIconText;
			if (_second > 0)
			{
				messageBoxPart.Second = _second;
				messageBoxPart.FengHaoBl = _fengHaoBl;
			}
			messageBoxPart.ShowInfo();
			messageBoxPart.ButtonClick = delegate(object s, EventArgs e)
			{
				int myMessageBoxPartReturn = messageBoxPart.MyMessageBoxPartReturn;
				messageBoxWindow.NotifyClose(myMessageBoxPartReturn);
				Super.m_MessageBoxPart = null;
				Super.CloseChildWindow(root, messageBoxWindow);
			};
			messageBoxWindow.SetContent(messageBoxWindow.BodyPresenter, messageBoxPart, 9.0, 0.0, true);
			root.Children.Add(messageBoxWindow);
			messageBoxWindow.X = (double)pos.x;
			messageBoxWindow.Y = (double)pos.y;
			return messageBoxWindow;
		}

		public static NoTitleWindow ShowDialogBox(Canvas root, int boxType, string message, int left = -1, int top = -1, int millisecs = 0, string name = "", string okName = "确定", string cancelName = "取消")
		{
			if (okName == Global.GetLang("确定"))
			{
				okName = Global.GetLang(Global.GetLang("确定"));
			}
			if (cancelName == Global.GetLang("取消"))
			{
				cancelName = Global.GetLang(Global.GetLang("取消"));
			}
			NoTitleWindow dialogBoxWindow = U3DUtils.NEW<NoTitleWindow>();
			Super.InitNoTitleWindow(dialogBoxWindow);
			MyDialogBoxPart dialogBoxPart = U3DUtils.NEW<MyDialogBoxPart>();
			dialogBoxPart.HintTitle = Global.GetLang("提示");
			dialogBoxPart.HintText = message;
			dialogBoxPart.BoxType = boxType;
			dialogBoxPart.OkText = okName;
			dialogBoxPart.CancelText = cancelName;
			dialogBoxPart.TimerClose(millisecs);
			dialogBoxPart.Name = name;
			dialogBoxPart.ButtonClick = delegate(object s, EventArgs e)
			{
				int myDialogBoxPartReturn = dialogBoxPart.MyDialogBoxPartReturn;
				dialogBoxWindow.NotifyClose(myDialogBoxPartReturn);
			};
			dialogBoxWindow.SetContent(dialogBoxWindow.BodyPresenter, dialogBoxPart, 2.0, 2.0);
			Canvas.SetZIndex(dialogBoxWindow, 9002.0);
			if (!string.IsNullOrEmpty(name))
			{
				dialogBoxWindow.Name = name;
			}
			root.Children.Add(dialogBoxWindow);
			return dialogBoxWindow;
		}

		public static void ShowDialogBox2(Canvas root, int boxType, string message, int millisecs = 0, string name = "")
		{
			NoTitleWindow dialogBoxWindow = Super.ShowDialogBox(root, 0, message, -1, -1, millisecs, name, "确定", "取消");
			dialogBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				Object.Destroy(dialogBoxWindow.gameObject);
				dialogBoxWindow = null;
				return true;
			};
		}

		public static void ShowDialogBoxEx(Canvas root, int boxType, string message, int left = -1, int top = -1, int millisecs = 0, string name = "")
		{
			NoTitleWindow dialogBoxWindow = Super.ShowDialogBox(root, boxType, message, left, top, millisecs, name, "确定", "取消");
			dialogBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				Object.Destroy(dialogBoxWindow.gameObject);
				dialogBoxWindow = null;
				return true;
			};
		}

		public static void CloseDialogBox(Canvas root, NoTitleWindow dialogBoxWindow)
		{
			if (dialogBoxWindow != null)
			{
				WindowManage.RemoveWindows(dialogBoxWindow);
				Object.Destroy(dialogBoxWindow.gameObject);
				dialogBoxWindow = null;
			}
		}

		public static string GetXapParamByName(string name, string defVal = "")
		{
			return Global.GetXapParamByName(name, defVal);
		}

		public static void ShowLoadingGame(Canvas root)
		{
			LoadingGame loadingGame = U3DUtils.NEW<LoadingGame>();
			loadingGame.NextStep = delegate(object s, NextStepEventArgs e)
			{
				Object.Destroy((s as LoadingGame).gameObject);
				Global.InitFilterFields();
				if (Global.GetLoginMode() == "0")
				{
					ToGame toGame = Super.ShowToGame(root);
					if (PlayerPrefs.GetInt("NoticeShowed") == 0 && PlayerPrefs.GetInt("NewLastServerInfoID") != 0)
					{
						Super.ShowNoticeWindow(root);
						PlayerPrefs.SetInt("NoticeShowed", 1);
					}
					string text = Global.StringReplaceAll(Super.GetXapParamByName("uid", string.Empty), ":", string.Empty);
					if ("-1" == text)
					{
						Global.InitFilterFields();
					}
				}
				else
				{
					Global.InitFilterFields();
					if (PlatSDKMgr.PlatName == "QQ" || PlatSDKMgr.PlatName == "YYB")
					{
						Super.ShowTencentLogin(root);
						if (PlayerPrefs.GetInt("NoticeShowed") == 0 && PlayerPrefs.GetInt("NewLastServerInfoID") != 0)
						{
							Super.ShowNoticeWindow(root);
							PlayerPrefs.SetInt("NoticeShowed", 1);
						}
						return;
					}
					Super.ShowPlatformUserLogin(root, true);
					if (PlayerPrefs.GetInt("NoticeShowed") == 0 && PlayerPrefs.GetInt("NewLastServerInfoID") != 0)
					{
						Super.ShowNoticeWindow(root);
						PlayerPrefs.SetInt("NoticeShowed", 1);
					}
					string userID = Global.Data.UserID;
					if (string.IsNullOrEmpty(userID) || userID.Equals("-1"))
					{
						Global.InitFilterFields();
						PlatSDKMgr.Login(null, string.Empty);
						if (!Context.IsHaiwai)
						{
							PlatSDKMgr.ActiveReport();
						}
					}
				}
			};
			root.Children.Add(loadingGame);
		}

		public static void ShowCheckingUpdateGame(Canvas root)
		{
			if (Super.CheckingGame == null)
			{
				Super.CheckingGame = U3DUtils.NEW<CheckingUpdateGame>();
				root.Children.Add(Super.CheckingGame);
			}
		}

		public static void DestroyCheckingUpdateGame()
		{
			if (Super.CheckingGame != null)
			{
				Object.Destroy(Super.CheckingGame.gameObject);
				Super.CheckingGame = null;
			}
		}

		public static void ShowUpdateGameForUpdate(Canvas root, int localAppVerCode, int remoteAppVerCode, string url, byte[] remoteVersionBytes, List<XElement> needUpdateList, List<string> needDeleteList)
		{
			if (Super.UpdateGameInstance == null)
			{
				Super.UpdateGameInstance = U3DUtils.NEW<UpdateGame>();
				Super.UpdateGameInstance.NextStep = delegate(object s, NextStepEventArgs e)
				{
					Object.Destroy((s as UpdateGame).gameObject);
					Super.UpdateGameInstance = null;
					Super.ShowLoadingGame(root);
				};
			}
			Super.UpdateGameInstance.LocalAppVerCode = localAppVerCode;
			Super.UpdateGameInstance.RemoteAppVerCode = remoteAppVerCode;
			Super.UpdateGameInstance.URL = url;
			Super.UpdateGameInstance.RemoteVersionBytes = remoteVersionBytes;
			Super.UpdateGameInstance.NeedToUpdateFileList = needUpdateList;
			Super.UpdateGameInstance.NeedToDeleteFileList = needDeleteList;
			Super.UpdateGameInstance.ShowUpdateGameInfo();
			if (((needUpdateList != null && needUpdateList.Count > 0) || (needDeleteList != null && needDeleteList.Count > 0) || localAppVerCode < remoteAppVerCode) && Super.UpdateGameInstance != null)
			{
				root.Children.Add(Super.UpdateGameInstance);
			}
		}

		public static void ShowUpdateGameForZIPUpdate(Canvas root, string url, List<XElement> needUpdateZIPList, List<string> needDeleteZIPList)
		{
			if (Super.UpdateGameInstance == null)
			{
				Super.UpdateGameInstance = U3DUtils.NEW<UpdateGame>();
				Super.UpdateGameInstance.NextStep = delegate(object s, NextStepEventArgs e)
				{
					Object.Destroy((s as UpdateGame).gameObject);
					Super.UpdateGameInstance = null;
					Super.ShowLoadingGame(root);
					MUDebug.Log<string>(new string[]
					{
						"ShowLoadingGame:" + Time.realtimeSinceStartup
					});
				};
			}
			Super.UpdateGameInstance.URL = url;
			Super.UpdateGameInstance.NeedToUpdateZIPFileList = needUpdateZIPList;
			Super.UpdateGameInstance.NeedToDeleteZIPFileList = needDeleteZIPList;
			Super.UpdateGameInstance.ShowUpdateZIPGameInfo();
			if (((needUpdateZIPList != null && needUpdateZIPList.Count > 0) || (needDeleteZIPList != null && needDeleteZIPList.Count > 0)) && Super.UpdateGameInstance != null)
			{
				root.Children.Add(Super.UpdateGameInstance);
			}
		}

		public static PlatformUserLogin ShowPlatformUserLogin(Canvas root, bool isShowSelectPlatform = true)
		{
			if (Super.platformLogin == null)
			{
				Super.platformLogin = U3DUtils.NEW<PlatformUserLogin>();
				Super.platformLogin.LoginGameToLineServer = delegate(object s, EventArgs e)
				{
					Super.platformLogin = null;
					Object.Destroy((s as PlatformUserLogin).gameObject);
					(s as PlatformUserLogin).DestroyLogin3Map();
					U3DUtils.ClearAll3DObjects(true, true);
					Super.FakeConnectToLineServer();
					Super.ShowRoleManager(root);
				};
				root.Children.Add(Super.platformLogin);
			}
			return Super.platformLogin;
		}

		public static TencentLogin ShowTencentLogin(Canvas root)
		{
			if (Super.tencentLogin == null)
			{
				Super.tencentLogin = U3DUtils.NEW<TencentLogin>();
				Super.tencentLogin.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					Super.tencentLogin = null;
					Object.Destroy((s as TencentLogin).gameObject);
				};
				root.Children.Add(Super.tencentLogin);
			}
			return Super.tencentLogin;
		}

		public static ToGame ShowToGame(Canvas root)
		{
			ToGame toGame = U3DUtils.NEW<ToGame>();
			toGame.RegAccount = delegate(object s, NextStepEventArgs e)
			{
				string loginMode = Global.GetLoginMode();
				if ("0" == loginMode)
				{
					Object.Destroy((s as ToGame).gameObject);
					Super.ShowUserLogin(root);
				}
			};
			toGame.NextStep = delegate(object s, NextStepEventArgs e)
			{
				string loginMode = Global.GetLoginMode();
				if ("0" == loginMode)
				{
					Object.Destroy((s as ToGame).gameObject);
					Super.ShowUserLogin(root);
				}
			};
			root.Children.Add(toGame);
			return toGame;
		}

		public static void ShowUserLogin(Canvas root)
		{
			if (Super.UserLoginInstance == null)
			{
				Super.UserLoginInstance = U3DUtils.NEW<UserLogin>();
				Super.UserLoginInstance.LoginGameToLineServer = delegate(object s, EventArgs e)
				{
					Super.UserLoginInstance = null;
					Object.Destroy((s as UserLogin).gameObject);
					(s as UserLogin).DestroyLogin3Map();
					U3DUtils.ClearAll3DObjects(true, true);
					Super.FakeConnectToLineServer();
					Super.ShowRoleManager(root);
				};
				root.Children.Add(Super.UserLoginInstance);
			}
		}

		public static void FakeConnectToLineServer()
		{
			string xapParamByName = Super.GetXapParamByName("serverip", "127.0.0.1");
			Global.LineDataList = new List<LineData>();
			LineData lineData = new LineData
			{
				LineID = 1,
				GameServerIP = xapParamByName,
				GameServerPort = Global.GetGameServerPort(),
				OnlineCount = 0
			};
			Global.LineDataList.Add(lineData);
			Global.CurrentListData = Global.LineDataList[0];
		}

		public static void ShowRoleManager(Canvas root)
		{
			Super.roleManager = U3DUtils.NEW<RoleManager>();
			Super.roleManager.DirectLogin = (-1 != Global.Data.RoleID);
			if (KuaFuLoginManager.DirectLogin())
			{
				Super.roleManager.DirectLogin = true;
			}
			Super.roleManager.StartGameByRole = delegate(object s, EventArgs e)
			{
				bool connectFailed = Super.roleManager.ConnectFailed;
				Object.Destroy(Super.roleManager.gameObject);
				U3DUtils.ClearAll3DObjects(true, true);
				if (!connectFailed)
				{
					Super.ConnectToGameServerFailed = false;
					Super.ShowLoadingMap(root, 2);
				}
				else
				{
					Super.ConnectToGameServerFailed = true;
					string loginMode = Global.GetLoginMode();
					if (loginMode == "0")
					{
						Super.ShowUserLogin(root);
					}
					else
					{
						Super.ShowPlatformUserLogin(root, true);
					}
				}
			};
			Super.roleManager.GoBackEvent = delegate(object s, EventArgs e)
			{
				try
				{
					Object.Destroy(Super.roleManager.gameObject);
				}
				catch (Exception ex)
				{
				}
				U3DUtils.ClearAll3DObjects(true, true);
				Super.ConnectToGameServerFailed = true;
				GameInstance.Game.Disconnect();
				GameInstance.CreateNewTCPGame();
				string loginMode = Global.GetLoginMode();
				if (loginMode == "0")
				{
					Super.ShowUserLogin(root);
				}
				else
				{
					Super.ShowPlatformUserLogin(root, true);
				}
			};
			root.Children.Add(Super.roleManager);
		}

		public static void DestroyRoleManagerAndBack()
		{
			if (Super.roleManager != null)
			{
				Super.roleManager.ClosedBySDK();
			}
		}

		public static void BackToLogin()
		{
			U3DUtils.ClearAll3DObjects(true, true);
			Super.ConnectToGameServerFailed = true;
			GameInstance.Game.Disconnect();
			GameInstance.CreateNewTCPGame();
			string loginMode = Global.GetLoginMode();
			if (loginMode == "0")
			{
				Super.ShowUserLogin(Super.MainWindowRoot);
			}
			else
			{
				Super.ShowPlatformUserLogin(Super.MainWindowRoot, true);
			}
		}

		public static void DestroyLoadingMap()
		{
			if (null == Super.CurrentLoadingMap)
			{
				return;
			}
			GameObject gameObject = Super.CurrentLoadingMap.gameObject;
			Super.CurrentLoadingMap.Remove(gameObject, true);
			Super.CurrentLoadingMap = null;
			Object.Destroy(gameObject);
		}

		public static void ShowLoadingMap(Canvas root, int loadType = 2)
		{
			LoadingMap loadingMap = U3DUtils.NEW<LoadingMap>();
			Super.DestroyLoadingMap();
			Super.CurrentLoadingMap = loadingMap;
			if (loadType == 2)
			{
				loadingMap.MapCode = Global.Data.roleData.MapCode;
			}
			loadingMap.WorkFinished = delegate(object s2, EventArgs e2)
			{
				Super.ShowGameManager(root);
				if (null != Global.BackgroundAudio43D)
				{
					if (!Global.Data.SysSetting.CloseGameMusic)
					{
						Global.BackgroundAudio43D.PlayAudio(ConfigSettings.GetMapMusicFileByCode(Global.Data.roleData.MapCode, true), true, false);
					}
					else
					{
						Global.BackgroundAudio43D.StopPlay();
					}
				}
			};
			loadingMap.LoadType = loadType;
			root.Children.Add(loadingMap);
		}

		public static void ShowGameManager(Canvas root)
		{
			GameManager gameManager = U3DUtils.NEW<GameManager>();
			root.Children.Add(gameManager);
			gameManager.Coordinate = new Point(0, 0);
			gameManager.Z = 0.0;
			Super.MainGameMgr = gameManager;
		}

		public static Vector2 GetMainUISize()
		{
			MyAnchorCamera component = Global.UICamera.GetComponent<MyAnchorCamera>();
			if (null != component)
			{
				return new Vector2(component.suitableUI_width, component.suitableUI_width * (float)Screen.height / (float)Screen.width);
			}
			return new Vector2(960f, 540f);
		}

		public static Vector2 GetScreenSize()
		{
			if (Super.mScreenWidth == 0f)
			{
				Super.mScreenWidth = (float)Screen.width;
			}
			if (Super.mScreenHeight == 0f)
			{
				Super.mScreenHeight = (float)Screen.height;
			}
			return new Vector2(Super.mScreenWidth, Super.mScreenHeight);
		}

		public static ParcelPart _ParcelPart { get; set; }

		public static ParcelPart _ParcelRebornPart { get; set; }

		public static bool HasGChildWindowShown()
		{
			if (null == Super.GData.PlayZoneRoot)
			{
				return false;
			}
			Transform transform = Super.GData.PlayZoneRoot.transform;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				if (transform.GetChild(i).gameObject.name == "GChildWindow(Clone)" && NGUITools.GetActive(transform.GetChild(i).gameObject))
				{
					return true;
				}
			}
			return false;
		}

		public static void HintMainText(string msg, int maxCount = 10, int singleMax = 3)
		{
			int num = -1;
			int hashCode = msg.GetHashCode();
			if (maxCount > 0 && Super.goodsHintDict.Count >= maxCount)
			{
				long num2 = long.MaxValue;
				if (!Super.goodsHintDict.ContainsKey(hashCode))
				{
					foreach (KeyValuePair<int, Queue<HintTextdata>> keyValuePair in Super.goodsHintDict)
					{
						HintTextdata hintTextdata = keyValuePair.Value.Peek();
						if (hintTextdata != null && hintTextdata.Ticks < num2)
						{
							num2 = hintTextdata.Ticks;
							Dictionary<int, Queue<HintTextdata>>.Enumerator enumerator;
							KeyValuePair<int, Queue<HintTextdata>> keyValuePair2 = enumerator.Current;
							num = keyValuePair2.Key;
						}
					}
					if (num2 < 9223372036854775807L)
					{
						Super.goodsHintDict.Remove(num);
					}
				}
			}
			Queue<HintTextdata> queue;
			if (!Super.goodsHintDict.TryGetValue(hashCode, ref queue))
			{
				queue = new Queue<HintTextdata>();
				Super.goodsHintDict.Add(hashCode, queue);
			}
			if (queue.Count >= singleMax)
			{
				queue.Dequeue();
			}
			queue.Enqueue(new HintTextdata(msg));
		}

		public static void HintNewGoodsText(GoodsData goodsData, int goodsCount, int hint = 0)
		{
			Super.PlayGoodsSound(goodsData.GoodsID);
			string text = UIHelper.FormatGoodsName(goodsData, false, false, false);
			if (hint > 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyBox, StringUtil.substitute(Global.GetLang("新物品到随身仓库【{0}】({1})"), new object[]
				{
					text,
					goodsCount
				}), 0, -1, -1, 0);
			}
			string msg = string.Format(Global.GetLang("您获得了: {0}x{1}"), text, goodsCount);
			Super.HintMainText(msg, 30, 3);
		}

		public static string GetIconCode(XElement xmlItem)
		{
			if (xmlItem == null)
			{
				return string.Empty;
			}
			string xelementAttributeStr = Global.GetXElementAttributeStr(xmlItem, "IconCode");
			if (string.IsNullOrEmpty(xelementAttributeStr))
			{
				xelementAttributeStr = Global.GetXElementAttributeStr(xmlItem, "ID");
			}
			return xelementAttributeStr;
		}

		public static string GetIconCode(GoodVO goodVO)
		{
			if (goodVO == null)
			{
				return string.Empty;
			}
			string text = goodVO.IconCode;
			if (string.IsNullOrEmpty(text))
			{
				text = goodVO.ID.ToString();
			}
			return text;
		}

		public static string GetIconCode(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			return Super.GetIconCode(goodsXmlNodeByID);
		}

		public static string GetWindowsBakImageURLFromName(string name)
		{
			return StringUtil.substitute("NetImages/GameRes/Images/Plate/{0}", new object[]
			{
				name
			});
		}

		public static string GetSkillImageURLFromIconCode(string skillCode, string prePath = "")
		{
			return StringUtil.substitute("{0}Images/Skill/{1}.png", new object[]
			{
				prePath,
				skillCode
			});
		}

		public static string GetGoodsImageURLFromIconCode(string iconCode, string prePath = "")
		{
			return StringUtil.substitute("{0}Images/Goods/{1}.png", new object[]
			{
				prePath,
				iconCode
			});
		}

		public static string GetGoodsImageURLFromIconCodeEx(string iconCode)
		{
			return StringUtil.substitute("NetImages/Sales/{0}.png", new object[]
			{
				iconCode
			});
		}

		public static string GetFuBenPreviewImageString(string iconCode)
		{
			return StringUtil.substitute("NetImages/GameRes/Images/Preview/{0}.png", new object[]
			{
				iconCode
			});
		}

		public static string GetFuBenPreviewImageString2(string iconCode)
		{
			return StringUtil.substitute("NetImages/GameRes/Images/Preview/{0}.jpg", new object[]
			{
				iconCode
			});
		}

		public static string GetTaskImageString(string iconCode)
		{
			return StringUtil.substitute("NetImages/GameRes/Images/Task/{0}.png", new object[]
			{
				iconCode
			});
		}

		public static string GetTaskImageString2(string iconCode)
		{
			return StringUtil.substitute("NetImages/GameRes/Images/Task/{0}.jpg", new object[]
			{
				iconCode
			});
		}

		public static BitmapData GetGoodsImageFromFile(string iconCode)
		{
			BitmapData bitmapData = null;
			try
			{
				bitmapData = Global.GetGameResImage(StringUtil.substitute("Images/Goods/{0}.png", new object[]
				{
					iconCode
				}));
				if (bitmapData == null)
				{
					bitmapData = Global.GetGameResImage(StringUtil.substitute("Images/Plate/default.png", new object[0]));
				}
				return bitmapData;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
				bitmapData = null;
			}
			try
			{
				bitmapData = Global.GetGameResImage(StringUtil.substitute("Images/Plate/default.png", new object[0]));
			}
			catch (Exception)
			{
				bitmapData = null;
			}
			return bitmapData;
		}

		public static BitmapData ConvertBitmapToGrayBitmap(BitmapData bitmapImage, bool toGray = true)
		{
			return bitmapImage;
		}

		public static void HideChildWindow(GChildWindow childWindow)
		{
			WindowManage.RemoveWindows(childWindow);
		}

		public static void CloseChildWindow(SpriteSL root, GChildWindow childWindow)
		{
			if (null == root)
			{
				root = childWindow.parent;
			}
			root.Remove(childWindow, true);
			if (!(childWindow.TitleText == "NPCDialog"))
			{
				if (!(childWindow.TitleText == "NPCDialog"))
				{
					WindowManage.RemoveWindows(childWindow);
				}
			}
			childWindow = null;
		}

		public static void CloseChildWindow(SpriteSL root, string titleText)
		{
			if (string.IsNullOrEmpty(titleText))
			{
				return;
			}
			if (null == root)
			{
				root = Super.GData.PlayZoneRoot;
			}
			if (null != root)
			{
				GChildWindow[] componentsInChildren = root.GetComponentsInChildren<GChildWindow>();
				if (componentsInChildren == null || componentsInChildren.Length == 0)
				{
					return;
				}
				foreach (GChildWindow gchildWindow in componentsInChildren)
				{
					if (gchildWindow.TitleText != null && gchildWindow.TitleText.CompareTo(titleText) == 0)
					{
						gchildWindow.ChildWindowClose(gchildWindow, EventArgs.Empty);
						Super.CloseChildWindow(null, gchildWindow);
					}
				}
			}
		}

		private static void _InitChildWindow(GChildWindow childWindow, string title, double closeButtonLeft, double closeButtonTop, int titleType)
		{
			childWindow.TitleText = title;
			WindowManage.AddWindows(childWindow, childWindow.IsShowModal, null);
		}

		public static void InitChildWindow(GChildWindow childWindow, string title)
		{
			Super._InitChildWindow(childWindow, title, 31.0, 10.0, 0);
		}

		public static void InitChildWindow1(GChildWindow childWindow, string title)
		{
			Super._InitChildWindow(childWindow, title, 25.0, 5.0, 1);
		}

		public static void InitChildWindow2(GChildWindow childWindow, string title)
		{
			Super._InitChildWindow(childWindow, title, 31.0, 10.0, 0);
		}

		public static void CloseNoTitleWindow(Canvas root, NoTitleWindow noTitleWindow)
		{
			root.Children.Remove(noTitleWindow, true);
			noTitleWindow = null;
			Super.GData.NoTitleWindowCount--;
			if (Super.GData.NoTitleWindowCount < 0)
			{
				Super.GData.NoTitleWindowCount = 0;
			}
		}

		public static void InitNoTitleWindow(NoTitleWindow noTitleWindow)
		{
			WindowManage.AddWindows(noTitleWindow, false, null);
			noTitleWindow.LeftBorderWidth = 10.0;
			noTitleWindow.LeftBorderHeight = noTitleWindow.BodyHeight - 20.0;
			noTitleWindow.RightBorderWidth = 10.0;
			noTitleWindow.RightBorderHeight = noTitleWindow.BodyHeight - 20.0;
			noTitleWindow.RightBorderLeft = noTitleWindow.BodyWidth - 10.0;
			noTitleWindow.BottomBorderWidth = noTitleWindow.BodyWidth - 20.0;
			noTitleWindow.BottomBorderHeight = 10.0;
			noTitleWindow.BottomBorderLeft = 10.0;
			noTitleWindow.BottomBorderTop = noTitleWindow.BodyHeight - 10.0;
			Super.GData.NoTitleWindowCount++;
		}

		public static void CloseNoBorderWindow(Canvas root, NoBorderWindow noBorderWindow)
		{
			WindowManage.RemoveWindows(noBorderWindow);
			root.Children.Remove(noBorderWindow, true);
			noBorderWindow.Visibility = false;
			noBorderWindow = null;
		}

		public static void InitNoBorderWindow(NoBorderWindow noBorderWindow)
		{
			WindowManage.AddWindows(noBorderWindow, false, null);
		}

		public static void CloseBitmapWindow(Canvas root, GBitmapWindow bitmapWindow)
		{
			root.Children.Remove(bitmapWindow, true);
			bitmapWindow.Visibility = false;
			bitmapWindow = null;
		}

		public static void InitBitmapWindow(GBitmapWindow bitmapWindow)
		{
			WindowManage.AddWindows(bitmapWindow, false, null);
		}

		public static int GetChildLeft(int parentWidth, int childWidth)
		{
			return (parentWidth - childWidth) / 2;
		}

		public static int GetChildTop(int parentHeight, int childHeight)
		{
			return (parentHeight - childHeight) / 2;
		}

		public static void SetChildCoordinate(int parentWidth, int parentHeight, GChildWindow targetWindow, GChildWindow childWindow)
		{
			targetWindow.Left = (double)((int)((double)parentWidth - childWindow.BodyWidth - targetWindow.BodyWidth) / 2);
			targetWindow.Top = (double)((int)((double)parentHeight - targetWindow.BodyHeight - 39.0) / 2);
			childWindow.Left = targetWindow.Left + targetWindow.BodyWidth;
			childWindow.Top = targetWindow.Top;
		}

		public static void ShowChangingMapWindow(Canvas parent)
		{
			if (null != Super.ChangingMapWindow)
			{
				return;
			}
			Super.ChangingMapWindow = new Canvas();
			Canvas.SetZIndex(Super.ChangingMapWindow, 1000000.0);
			parent.Children.Add(Super.ChangingMapWindow);
		}

		public static void HideChangingMapWindow(Canvas parent)
		{
			if (null != Super.ChangingMapWindow)
			{
				parent.Children.Remove(Super.ChangingMapWindow, true);
				Super.ChangingMapWindow = null;
			}
			Super.HideNetWaiting();
		}

		public static int FindEquipBagIndex(int categoriy)
		{
			if (GTipServiceEx.HandValue == 0)
			{
				return 1;
			}
			if (GTipServiceEx.HandValue == 1)
			{
				return 0;
			}
			if (categoriy == 6)
			{
				if (Super.FindUsingEuip(categoriy, 0) == null)
				{
					return 0;
				}
				if (Super.FindUsingEuip(categoriy, 1) == null)
				{
					return 1;
				}
			}
			if (categoriy == 36)
			{
				if (Super.FindChongShengUsingEuip(categoriy, 0) == null)
				{
					return 0;
				}
				if (Super.FindChongShengUsingEuip(categoriy, 1) == null)
				{
					return 1;
				}
			}
			else if (categoriy == 18)
			{
				if (Super.FindUsingEuip(categoriy, 0) == null)
				{
					return 1;
				}
				if (Super.FindUsingEuip(categoriy, 1) == null)
				{
					return 0;
				}
			}
			else
			{
				if (Super.FindUsingEuip(categoriy, 1) == null)
				{
					return 0;
				}
				if (Super.FindUsingEuip(categoriy, 0) == null)
				{
					return 1;
				}
			}
			return 0;
		}

		public static void EquipAction(GoodsData goodsData, ZhuangBeiPart zhuangBeiPart)
		{
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
			if (25 <= goodsCatetoriy && 27 >= goodsCatetoriy)
			{
				return;
			}
			if (25 > goodsCatetoriy && 0 <= goodsCatetoriy)
			{
				if (goodsData.Using > 0)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
					if (goodsXmlNodeByID == null)
					{
						return;
					}
					int handType = goodsXmlNodeByID.HandType;
					GGoodIcon icon = null;
					icon = U3DUtils.NEW<GGoodIcon>();
					icon.isAutoSize = true;
					icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Equip/{0}.png", new object[]
					{
						Super.GetIconCode(goodsXmlNodeByID)
					}), false, 0);
					icon.TipType = 1;
					icon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
					{
						goodsXmlNodeByID.ID,
						1,
						goodsData.Id,
						0
					});
					icon.ItemCategory = goodsXmlNodeByID.Categoriy;
					icon.ItemCode = goodsData.GoodsID;
					icon.ItemObject = goodsData;
					icon.BoxTypes = 0;
					icon.TextSize = 20;
					icon.TextShadowColor = 4278190080U;
					if (goodsData.ExcellenceInfo > 0 || goodsCatetoriy == 22 || goodsCatetoriy == 9 || goodsCatetoriy == 10 || goodsCatetoriy == 23 || goodsCatetoriy == 24)
					{
						zhuangBeiPart.SetExcellenceStat(icon, goodsCatetoriy);
					}
					zhuangBeiPart.SetEquipBorderBySuitID(icon, goodsData);
					icon.addEventListener("click", new MouseEventHandler(zhuangBeiPart.MouseLeftButtonUp));
					icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
					{
						if (ev.IDType == 2)
						{
							if (Global.Data.GameScene.IsDead())
							{
								return;
							}
							GoodsData goodsData2 = icon.ItemObject as GoodsData;
							if (Global.CanAddGoods(goodsData2.GoodsID, goodsData2.GCount, goodsData2.Binding, goodsData2.Endtime, false))
							{
								if (goodsData2.Using == 1)
								{
									goodsData2.Using = 0;
									GameInstance.Game.SpriteModGoods(2, goodsData2.Id, goodsData2.GoodsID, goodsData2.Using, goodsData2.Site, goodsData2.GCount, goodsData2.BagIndex, string.Empty);
								}
							}
							else
							{
								GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再卸载装备..."), new object[0]), 1, -1, -1, 0);
							}
						}
						else if (ev.IDType == 16)
						{
							zhuangBeiPart.DPSelectedItem(zhuangBeiPart, new DPSelectedItemEventArgs
							{
								ID = -10
							});
							PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
							{
								ID = 1330
							});
						}
					};
					icon.DPImageDownloadedItem = delegate(object s, DPSelectedItemEventArgs ev)
					{
						zhuangBeiPart.SetBoxCollider(icon);
					};
					zhuangBeiPart.SetZhuangBeiPeiDai(icon, goodsCatetoriy, handType, goodsData.BagIndex);
				}
				else
				{
					GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
					if (goodsXmlNodeByID2 != null)
					{
						int handType2 = goodsXmlNodeByID2.HandType;
						int num = goodsCatetoriy;
						if (goodsCatetoriy >= 11 && goodsCatetoriy <= 21)
						{
							if (handType2 == 1)
							{
								num = 11;
							}
							else if (handType2 == 0)
							{
								num = 36;
							}
							else if (handType2 == 2)
							{
								if (goodsData.BagIndex == 0)
								{
									num = 11;
								}
								else if (goodsData.BagIndex == 1)
								{
									num = 36;
								}
							}
							if (zhuangBeiPart.equipIcon[num].Count() > 0)
							{
								zhuangBeiPart.equipIcon[num].RemoveAt(0, true, true);
							}
						}
						else if (goodsCatetoriy == 6)
						{
							if (goodsData.BagIndex == 0)
							{
								num = goodsCatetoriy;
							}
							else if (goodsData.BagIndex == 1)
							{
								num = 25 + goodsCatetoriy;
							}
							if (zhuangBeiPart.equipIcon[num].Count() > 0)
							{
								zhuangBeiPart.equipIcon[num].RemoveAt(0, true, true);
							}
						}
						else if (goodsCatetoriy == 10 || goodsCatetoriy == 9)
						{
							if (zhuangBeiPart.equipIcon[9].Count() > 0)
							{
								GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(zhuangBeiPart.equipIcon[9].gameObject);
								if (ggoodIcon.EnableHint)
								{
									ggoodIcon.EnableHint = false;
								}
								zhuangBeiPart.equipIcon[9].RemoveAt(0, true, true);
							}
						}
						else if (zhuangBeiPart.equipIcon[num].Count() > 0)
						{
							GGoodIcon ggoodIcon2 = U3DUtils.AS<GGoodIcon>(zhuangBeiPart.equipIcon[num].gameObject);
							if (ggoodIcon2.EnableHint)
							{
								ggoodIcon2.EnableHint = false;
							}
							zhuangBeiPart.equipIcon[num].RemoveAt(0, true, true);
						}
					}
				}
				if (zhuangBeiPart != null)
				{
					zhuangBeiPart.RefreshBufferUI();
				}
			}
		}

		public static GoodsData FindUsingEuip(int categoriy, int handType)
		{
			if (Super.GData.RoleUsingGoodsDataList == null)
			{
				return null;
			}
			foreach (KeyValuePair<int, GoodsData> keyValuePair in Super.GData.RoleUsingGoodsDataList)
			{
				GoodsData value = keyValuePair.Value;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
				if (goodsXmlNodeByID != null)
				{
					int categoriy2 = goodsXmlNodeByID.Categoriy;
					int actionType = goodsXmlNodeByID.ActionType;
					int num;
					if (Global.Data.roleData.Occupation == 3 && goodsXmlNodeByID.ActionType == 1 && categoriy2 == 17)
					{
						num = 2;
					}
					else
					{
						num = goodsXmlNodeByID.HandType;
					}
					if (categoriy >= 11 && categoriy <= 21)
					{
						if (categoriy2 >= 11 && categoriy2 <= 21)
						{
							if (num == handType)
							{
								return value;
							}
							if (num == 2)
							{
								if (handType == 0)
								{
									if (value.BagIndex == 1)
									{
										return value;
									}
								}
								else if (handType == 1 && value.BagIndex == 0)
								{
									return value;
								}
							}
						}
					}
					else if (categoriy == 6)
					{
						if (categoriy2 == 6)
						{
							if (handType == 0)
							{
								if (value.BagIndex == 0)
								{
									return value;
								}
							}
							else if (handType == 1 && value.BagIndex == 1)
							{
								return value;
							}
						}
					}
					else
					{
						if (categoriy == 10 || categoriy == 9)
						{
							return value;
						}
						if (categoriy == categoriy2)
						{
							return value;
						}
					}
				}
			}
			return null;
		}

		public static List<GoodsData> FindWuQi(int equipCategory, int actionType = -1, int handType = -1)
		{
			List<GoodsData> list = new List<GoodsData>();
			GoodsData goodsData = Super.FindUsingEuip(equipCategory, 0);
			GoodsData goodsData2 = Super.FindUsingEuip(equipCategory, 1);
			if (equipCategory >= 11 && equipCategory <= 21)
			{
				if (equipCategory == 11 || equipCategory == 12 || equipCategory == 13 || equipCategory == 16 || equipCategory == 19)
				{
					if (actionType == 1)
					{
						bool flag = false;
						bool flag2 = false;
						if (goodsData != null)
						{
							GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
							int categoriy = goodsXmlNodeByID.Categoriy;
							int num = goodsXmlNodeByID.HandType;
							if (Global.Data.roleData.Occupation == 3 && goodsXmlNodeByID.ActionType == 1 && goodsXmlNodeByID.Categoriy == 17)
							{
								num = 2;
							}
							if (categoriy != 11 || categoriy != 12 || categoriy != 13 || categoriy != 16 || categoriy != 19)
							{
								if (categoriy != 18)
								{
									if (num != 2)
									{
										list.Add(goodsData);
									}
									else
									{
										flag = true;
									}
								}
								else if (GTipServiceEx.HandValue == 0)
								{
									list.Add(goodsData);
								}
								else
								{
									flag = true;
								}
							}
						}
						if (goodsData2 != null)
						{
							GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
							int categoriy2 = goodsXmlNodeByID2.Categoriy;
							int num2 = goodsXmlNodeByID2.HandType;
							int actionType2 = goodsXmlNodeByID2.ActionType;
							if (Global.Data.roleData.Occupation == 3 && goodsXmlNodeByID2.ActionType == 1 && goodsXmlNodeByID2.Categoriy == 17)
							{
								num2 = 2;
							}
							if (actionType2 == 2)
							{
								list.Add(goodsData2);
							}
							else if ((categoriy2 != 11 || categoriy2 != 12 || categoriy2 != 13 || categoriy2 != 16 || categoriy2 != 19) && categoriy2 != 18)
							{
								if (num2 != 2)
								{
									list.Add(goodsData2);
								}
								else
								{
									flag2 = true;
								}
							}
						}
						if (flag && flag2)
						{
							if (GTipServiceEx.HandValue == 0)
							{
								list.Add(goodsData);
							}
							else
							{
								list.Add(goodsData2);
							}
						}
					}
					else if (actionType == 2 || actionType == 5 || actionType == 7)
					{
						if (goodsData != null)
						{
							list.Add(goodsData);
						}
						if (goodsData2 != null)
						{
							list.Add(goodsData2);
						}
					}
				}
				else if (equipCategory != 12)
				{
					if (equipCategory != 13)
					{
						if (equipCategory == 14)
						{
							if (actionType == 3)
							{
								if (goodsData != null)
								{
									list.Add(goodsData);
								}
								if (goodsData2 != null)
								{
									GoodVO goodsXmlNodeByID3 = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
									int categoriy3 = goodsXmlNodeByID3.Categoriy;
									if (categoriy3 != 20)
									{
										list.Add(goodsData2);
									}
								}
							}
						}
						else if (equipCategory == 15)
						{
							if (actionType == 4)
							{
								if (goodsData != null)
								{
									GoodVO goodsXmlNodeByID4 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
									int categoriy4 = goodsXmlNodeByID4.Categoriy;
									if (categoriy4 != 21)
									{
										list.Add(goodsData);
									}
								}
								if (goodsData2 != null)
								{
									list.Add(goodsData2);
								}
							}
						}
						else if (equipCategory != 16)
						{
							if (equipCategory == 17)
							{
								if (actionType == 1)
								{
									if (Global.Data.roleData.Occupation == 3)
									{
										bool flag3 = false;
										bool flag4 = false;
										if (goodsData != null)
										{
											GoodVO goodsXmlNodeByID5 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
											int categoriy5 = goodsXmlNodeByID5.Categoriy;
											int num3 = goodsXmlNodeByID5.HandType;
											if (goodsXmlNodeByID5.Categoriy == 17 && goodsXmlNodeByID5.ActionType == 1)
											{
												num3 = 2;
											}
											if (categoriy5 != 18)
											{
												if (num3 != 2)
												{
													list.Add(goodsData);
												}
												else
												{
													flag3 = true;
												}
											}
											else if (GTipServiceEx.HandValue == 0)
											{
												list.Add(goodsData);
											}
											else
											{
												flag3 = true;
											}
										}
										if (goodsData2 != null)
										{
											GoodVO goodsXmlNodeByID6 = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
											int categoriy6 = goodsXmlNodeByID6.Categoriy;
											int num4 = goodsXmlNodeByID6.HandType;
											int actionType3 = goodsXmlNodeByID6.ActionType;
											if (goodsXmlNodeByID6.Categoriy == 17 && goodsXmlNodeByID6.ActionType == 1)
											{
												num4 = 2;
											}
											if (actionType3 == 2)
											{
												list.Add(goodsData2);
											}
											else if ((categoriy6 != 11 || categoriy6 != 12 || categoriy6 != 13 || categoriy6 != 16 || categoriy6 != 19) && categoriy6 != 18)
											{
												if (num4 != 2)
												{
													list.Add(goodsData2);
												}
												else
												{
													flag4 = true;
												}
											}
										}
										if (flag3 && flag4)
										{
											if (GTipServiceEx.HandValue == 0)
											{
												list.Add(goodsData);
											}
											else
											{
												list.Add(goodsData2);
											}
										}
									}
									else
									{
										if (goodsData != null)
										{
											GoodVO goodsXmlNodeByID7 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
											int categoriy7 = goodsXmlNodeByID7.Categoriy;
											if (categoriy7 != 18)
											{
												list.Add(goodsData);
											}
										}
										if (goodsData2 != null)
										{
											list.Add(goodsData2);
										}
									}
								}
								else if (actionType == 2 || actionType == 5 || actionType == 7)
								{
									if (goodsData != null)
									{
										list.Add(goodsData);
									}
									if (goodsData2 != null)
									{
										list.Add(goodsData2);
									}
								}
							}
							else if (equipCategory == 18)
							{
								if (actionType == 1)
								{
									if (goodsData != null)
									{
										list.Add(goodsData);
									}
									if (goodsData2 != null)
									{
										GoodVO goodsXmlNodeByID8 = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
										int categoriy8 = goodsXmlNodeByID8.Categoriy;
										int handType2 = goodsXmlNodeByID8.HandType;
										int actionType4 = goodsXmlNodeByID8.ActionType;
										if (actionType4 == 2)
										{
											list.Add(goodsData2);
										}
										else if ((categoriy8 != 11 || categoriy8 != 12 || categoriy8 != 13 || categoriy8 != 16 || categoriy8 != 19) && categoriy8 != 17 && handType2 != 2)
										{
											list.Add(goodsData2);
										}
									}
								}
							}
							else if (equipCategory != 19)
							{
								if (equipCategory == 20)
								{
									if (actionType == 1)
									{
										if (goodsData != null)
										{
											GoodVO goodsXmlNodeByID9 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
											int categoriy9 = goodsXmlNodeByID9.Categoriy;
											if (categoriy9 != 14)
											{
												list.Add(goodsData);
											}
										}
										if (goodsData2 != null)
										{
											list.Add(goodsData2);
										}
									}
								}
								else if (equipCategory == 21 && actionType == 1)
								{
									if (goodsData != null)
									{
										list.Add(goodsData);
									}
									if (goodsData2 != null)
									{
										GoodVO goodsXmlNodeByID10 = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
										int categoriy10 = goodsXmlNodeByID10.Categoriy;
										int actionType5 = goodsXmlNodeByID10.ActionType;
										if (actionType5 == 2)
										{
											list.Add(goodsData2);
										}
										else if (categoriy10 != 15)
										{
											list.Add(goodsData2);
										}
									}
								}
							}
						}
					}
				}
			}
			return list;
		}

		public static List<GoodsData> FindChongShengEquip(int equipCategory)
		{
			List<GoodsData> list = new List<GoodsData>();
			if (equipCategory != 36)
			{
				GoodsData goodsData = Super.FindChongShengUsingEuip(equipCategory, -1);
				if (goodsData != null)
				{
					list.Add(goodsData);
				}
				return list;
			}
			GoodsData goodsData2 = Super.FindChongShengUsingEuip(equipCategory, 0);
			GoodsData goodsData3 = Super.FindChongShengUsingEuip(equipCategory, 1);
			if (goodsData2 == null || goodsData3 == null)
			{
				return list;
			}
			if (goodsData2 != null && goodsData3 != null)
			{
				if (GTipServiceEx.HandValue == 0)
				{
					list.Add(goodsData3);
				}
				else
				{
					list.Add(goodsData2);
				}
				return list;
			}
			return list;
		}

		public static GoodsData FindChongShengUsingEuip(int categoriy, int handType)
		{
			if (Super.GData.RoleUsingChongShengGoodsDataList == null)
			{
				return null;
			}
			foreach (KeyValuePair<int, GoodsData> keyValuePair in Super.GData.RoleUsingChongShengGoodsDataList)
			{
				GoodsData value = keyValuePair.Value;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
				if (goodsXmlNodeByID != null)
				{
					int categoriy2 = goodsXmlNodeByID.Categoriy;
					int actionType = goodsXmlNodeByID.ActionType;
					if (categoriy == 36)
					{
						if (categoriy2 == 36)
						{
							if (handType == 0)
							{
								if (value.BagIndex == 0)
								{
									return value;
								}
							}
							else if (handType == 1 && value.BagIndex == 1)
							{
								return value;
							}
						}
					}
					else if (categoriy == categoriy2)
					{
						return value;
					}
				}
			}
			return null;
		}

		public static List<GoodsData> FindEquip(int equipCategory)
		{
			List<GoodsData> list = new List<GoodsData>();
			if (equipCategory != 6)
			{
				GoodsData goodsData = Super._FindEquip(equipCategory, -1);
				if (goodsData != null)
				{
					list.Add(goodsData);
				}
				return list;
			}
			GoodsData goodsData2 = Super.FindUsingEuip(equipCategory, 0);
			GoodsData goodsData3 = Super.FindUsingEuip(equipCategory, 1);
			if (goodsData2 == null || goodsData3 == null)
			{
				return list;
			}
			if (goodsData2 != null && goodsData3 != null)
			{
				if (GTipServiceEx.HandValue == 0)
				{
					list.Add(goodsData3);
				}
				else
				{
					list.Add(goodsData2);
				}
				return list;
			}
			return list;
		}

		public static GoodsData _FindEquip(int equipCategory, int bagIndex = -1)
		{
			Dictionary<int, GoodsData> dictionary;
			if (Global.IsRebornEquip(equipCategory))
			{
				dictionary = Super.GData.RoleUsingChongShengGoodsDataList;
			}
			else
			{
				dictionary = Super.GData.RoleUsingGoodsDataList;
			}
			if (dictionary == null)
			{
				return null;
			}
			foreach (KeyValuePair<int, GoodsData> keyValuePair in dictionary)
			{
				GoodsData value = keyValuePair.Value;
				if (value != null)
				{
					if (value.Using > 0)
					{
						if (bagIndex < 0 || value.BagIndex == bagIndex)
						{
							GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
							if (goodsXmlNodeByID != null)
							{
								int num = goodsXmlNodeByID.Categoriy;
								if (equipCategory == 10)
								{
									equipCategory = 9;
								}
								if (num == 10)
								{
									num = 9;
								}
								if (equipCategory == num)
								{
									return value;
								}
							}
						}
					}
				}
			}
			return null;
		}

		public static GoodsData GetGoodsOnBody(int goodsID, int goodsDbID, int goodsOwnerType, int excludeGoodsDbId = -1)
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return null;
			}
			if (goodsOwnerType == 0)
			{
				GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(goodsDbID, null);
				if (goodsDataByDbID != null && goodsDataByDbID.Using > 0)
				{
					return null;
				}
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return null;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy >= 25)
			{
				return null;
			}
			if (goodsXmlNodeByID.ToOccupation != -1)
			{
			}
			int num = -1;
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (num < 0 || Global.Data.roleData.GoodsDataList[i].BagIndex == num)
				{
					if (Global.Data.roleData.GoodsDataList[i].Id != goodsDbID && Global.Data.roleData.GoodsDataList[i].Using > 0)
					{
						GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.roleData.GoodsDataList[i].GoodsID);
						if (goodsXmlNodeByID2 != null)
						{
							int categoriy2 = goodsXmlNodeByID2.Categoriy;
							if (categoriy == categoriy2)
							{
								if (excludeGoodsDbId != Global.Data.roleData.GoodsDataList[i].Id)
								{
									return Global.Data.roleData.GoodsDataList[i];
								}
							}
						}
					}
				}
			}
			return null;
		}

		public static int CanOpenUpLevelGiftBag()
		{
			if (Global.Data.roleData == null)
			{
				return -1;
			}
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return -1;
			}
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.roleData.GoodsDataList[i].GoodsID);
				if (goodsXmlNodeByID != null)
				{
					int categoriy = goodsXmlNodeByID.Categoriy;
					if (categoriy == 302)
					{
						int toOccupation = goodsXmlNodeByID.ToOccupation;
						int toLevel = goodsXmlNodeByID.ToLevel;
						if (Global.ValidOccupation(toOccupation, -1) && toLevel <= Global.Data.roleData.Level)
						{
							return Global.Data.roleData.GoodsDataList[i].Id;
						}
					}
				}
			}
			return -1;
		}

		public static TaskData GetTaskDataByTaskID(int taskID)
		{
			if (Global.Data.roleData.TaskDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Global.Data.roleData.TaskDataList.Count; i++)
			{
				if (Global.Data.roleData.TaskDataList[i].DoingTaskID == taskID)
				{
					return Global.Data.roleData.TaskDataList[i];
				}
			}
			return null;
		}

		public static TaskData GetTaskDataByDbID(int dbID)
		{
			if (Global.Data.roleData.TaskDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Global.Data.roleData.TaskDataList.Count; i++)
			{
				if (Global.Data.roleData.TaskDataList[i].DbID == dbID)
				{
					return Global.Data.roleData.TaskDataList[i];
				}
			}
			return null;
		}

		public static string GetTaskGoodsName(XElement taskXml, int num)
		{
			string empty = string.Empty;
			int num2 = 0;
			int num3 = 0;
			Global.ParsePropNameInfo(Global.GetXElementAttributeStr(taskXml, StringUtil.substitute("PropsName{0}", new object[]
			{
				num
			})), out empty, out num2, out num3);
			return empty;
		}

		public static string GetTaskGoodsName(TaskVO taskVO, int num)
		{
			string empty = string.Empty;
			int num2 = 0;
			int num3 = 0;
			if (num == 1)
			{
				Global.ParsePropNameInfo(taskVO.PropsName1, out empty, out num2, out num3);
			}
			else if (num == 2)
			{
				Global.ParsePropNameInfo(taskVO.PropsName2, out empty, out num2, out num3);
			}
			return empty;
		}

		public static string GetTaskTargetName(XElement taskXml, int num)
		{
			string result = string.Empty;
			int xelementAttributeInt = Global.GetXElementAttributeInt(taskXml, StringUtil.substitute("TargetType{0}", new object[]
			{
				num
			}));
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(taskXml, StringUtil.substitute("TargetNPC{0}", new object[]
			{
				num
			}));
			if (xelementAttributeInt == 0)
			{
				if (xelementAttributeInt2 != -1)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt2);
					result = npcvobyID.SName;
				}
			}
			else if (xelementAttributeInt == 1)
			{
				if (xelementAttributeInt2 != -1)
				{
					MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(xelementAttributeInt2);
					result = monsterXmlNodeByID.SName;
				}
			}
			else if (xelementAttributeInt == 2)
			{
				if (xelementAttributeInt2 != -1)
				{
					MonsterVO monsterXmlNodeByID2 = ConfigMonsters.GetMonsterXmlNodeByID(xelementAttributeInt2);
					result = monsterXmlNodeByID2.SName;
				}
			}
			else if (xelementAttributeInt == 11)
			{
				if (xelementAttributeInt2 != -1)
				{
					result = "KillMonsterForLevel";
				}
			}
			else if (xelementAttributeInt == 7)
			{
				if (xelementAttributeInt2 != -1)
				{
					NPCInfoVO npcvobyID2 = ConfigNPCs.GetNPCVOByID(xelementAttributeInt2);
					result = npcvobyID2.SName;
				}
			}
			else if (xelementAttributeInt == 8)
			{
				if (xelementAttributeInt2 != -1)
				{
					MonsterVO monsterXmlNodeByID3 = ConfigMonsters.GetMonsterXmlNodeByID(xelementAttributeInt2);
					result = monsterXmlNodeByID3.SName;
				}
			}
			else if (xelementAttributeInt == 9)
			{
				if (xelementAttributeInt2 != -1)
				{
					NPCInfoVO npcvobyID3 = ConfigNPCs.GetNPCVOByID(xelementAttributeInt2);
					result = npcvobyID3.SName;
				}
			}
			else if (xelementAttributeInt == 10)
			{
				if (xelementAttributeInt2 != -1)
				{
					NPCInfoVO npcvobyID4 = ConfigNPCs.GetNPCVOByID(xelementAttributeInt2);
					result = npcvobyID4.SName;
				}
			}
			else if (199 < xelementAttributeInt && xelementAttributeInt < 228)
			{
				result = string.Empty;
			}
			else if (xelementAttributeInt == 13)
			{
				result = Global.GetLang("通关副本");
			}
			else if (xelementAttributeInt != 4)
			{
				NPCInfoVO npcvobyID5 = ConfigNPCs.GetNPCVOByID(xelementAttributeInt2);
				result = npcvobyID5.SName;
			}
			return result;
		}

		public static string GetTaskTargetName(TaskVO taskVO, int num)
		{
			string result = string.Empty;
			int num2 = 0;
			int num3 = 0;
			if (num == 1)
			{
				num2 = taskVO.TargetType1;
				num3 = taskVO.TargetNPC1;
			}
			else if (num == 2)
			{
				num2 = taskVO.TargetType2;
				num3 = taskVO.TargetNPC2;
			}
			else if (num == 0)
			{
				num3 = -1;
			}
			if (num2 == 0)
			{
				if (num3 != -1)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(num3);
					result = npcvobyID.SName;
				}
			}
			else if (num2 == 1)
			{
				if (num3 != -1)
				{
					MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(num3);
					result = monsterXmlNodeByID.SName;
				}
			}
			else if (num2 == 2)
			{
				if (num3 != -1)
				{
					MonsterVO monsterXmlNodeByID2 = ConfigMonsters.GetMonsterXmlNodeByID(num3);
					result = monsterXmlNodeByID2.SName;
				}
			}
			else if (num2 == 11)
			{
				if (num3 != -1)
				{
					result = "KillMonsterForLevel";
				}
			}
			else if (num2 == 7)
			{
				if (num3 != -1)
				{
					NPCInfoVO npcvobyID2 = ConfigNPCs.GetNPCVOByID(num3);
					result = npcvobyID2.SName;
				}
			}
			else if (num2 == 8)
			{
				if (num3 != -1)
				{
					NPCInfoVO npcvobyID3 = ConfigNPCs.GetNPCVOByID(num3);
					result = npcvobyID3.SName;
				}
			}
			else if (num2 == 9)
			{
				if (num3 != -1)
				{
					NPCInfoVO npcvobyID4 = ConfigNPCs.GetNPCVOByID(num3);
					result = npcvobyID4.SName;
				}
			}
			else if (num2 == 10)
			{
				if (num3 != -1)
				{
					NPCInfoVO npcvobyID5 = ConfigNPCs.GetNPCVOByID(num3);
					result = npcvobyID5.SName;
				}
			}
			else if (199 < num2 && num2 < 228)
			{
				result = string.Empty;
			}
			else if (num2 == 13)
			{
				result = Global.GetLang("通关副本");
			}
			else if (num2 != 4)
			{
				NPCInfoVO npcvobyID6 = ConfigNPCs.GetNPCVOByID(num3);
				result = npcvobyID6.SName;
			}
			return result;
		}

		public static int GetTaskTargetType(XElement taskXml, int num)
		{
			return Global.GetXElementAttributeInt(taskXml, StringUtil.substitute("TargetType{0}", new object[]
			{
				num
			}));
		}

		public static int GetTaskTargetType(TaskVO taskVO, int num)
		{
			int result = 0;
			if (num == 1)
			{
				result = taskVO.TargetType1;
			}
			else if (num == 2)
			{
				result = taskVO.TargetType2;
			}
			return result;
		}

		private static void ProcessFuBenNPCOrMonster(XElement taskXml, int currentMapCode, ref int isFuBen, ref int mapCode, ref int npcType, ref int npcID)
		{
			isFuBen = 0;
			if (currentMapCode == -1)
			{
				return;
			}
			if (Global.GetMapType(currentMapCode) == MapTypes.Normal)
			{
				return;
			}
			isFuBen = 1;
			Super.GetTaskDestNPCID(taskXml, ref mapCode, ref npcType, ref npcID);
		}

		private static void ProcessFuBenNPCOrMonster(TaskVO taskVO, int currentMapCode, ref int isFuBen, ref int mapCode, ref int npcType, ref int npcID)
		{
			isFuBen = 0;
			if (currentMapCode == -1)
			{
				return;
			}
			if (Global.GetMapType(currentMapCode) == MapTypes.Normal)
			{
				return;
			}
			isFuBen = 1;
			Super.GetTaskDestNPCID(taskVO, ref mapCode, ref npcType, ref npcID);
		}

		public static bool GetTaskTargetInfo(XElement taskXml, int num, TaskTargetInfo targetInfo)
		{
			int mapCode = -1;
			int npctype = -1;
			int npcID = -1;
			int toPosX = -1;
			int toPosY = -1;
			int isFuBen;
			int targetType;
			bool taskTargetID = Super.GetTaskTargetID(taskXml, num, out isFuBen, out mapCode, out npctype, out npcID, out targetType, false, out toPosX, out toPosY);
			if (taskTargetID)
			{
				targetInfo.NpcID = npcID;
				targetInfo.ToPosX = toPosX;
				targetInfo.ToPosY = toPosY;
				targetInfo.IsFuBen = isFuBen;
				targetInfo.NPCType = npctype;
				targetInfo.MapCode = mapCode;
				targetInfo.TargetType = targetType;
			}
			return taskTargetID;
		}

		public static bool GetTaskTargetID(XElement taskXml, int num, out int isFuBen, out int mapCode, out int npcType, out int npcID, out int targetType, bool replaceFuBen, out int posX, out int posY)
		{
			isFuBen = 0;
			mapCode = -1;
			npcType = 3;
			npcID = -1;
			posX = -1;
			posY = -1;
			targetType = Global.GetXElementAttributeInt(taskXml, StringUtil.substitute("TargetType{0}", new object[]
			{
				num
			}));
			int xelementAttributeInt = Global.GetXElementAttributeInt(taskXml, StringUtil.substitute("TargetNPC{0}", new object[]
			{
				num
			}));
			if (targetType == 0)
			{
				if (xelementAttributeInt != -1)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
					mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID.MapCode);
					npcType = 3;
					npcID = npcvobyID.ID;
				}
			}
			else if (targetType == 1)
			{
				if (xelementAttributeInt != -1)
				{
					MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(xelementAttributeInt);
					mapCode = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID.MapCode);
					npcType = 2;
					npcID = monsterXmlNodeByID.ID;
				}
			}
			else if (targetType == 11)
			{
				if (xelementAttributeInt != -1)
				{
					mapCode = Global.GetXElementAttributeInt(taskXml, "TargetMapCode1");
					npcType = 11;
					string xelementAttributeStr = Global.GetXElementAttributeStr(taskXml, "TargetPos1");
					if (!string.IsNullOrEmpty(xelementAttributeStr))
					{
						string[] array = xelementAttributeStr.Split(new char[]
						{
							','
						});
						if (array.Length == 2)
						{
							posX = array[0].SafeToInt32(0);
							posY = array[1].SafeToInt32(0);
						}
					}
				}
			}
			else if (targetType == 2)
			{
				if (xelementAttributeInt != -1)
				{
					MonsterVO monsterXmlNodeByID2 = ConfigMonsters.GetMonsterXmlNodeByID(xelementAttributeInt);
					mapCode = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID2.MapCode);
					npcType = 2;
					npcID = monsterXmlNodeByID2.ID;
				}
			}
			else if (targetType != 4)
			{
				if (targetType == 5)
				{
					if (xelementAttributeInt != -1)
					{
						NPCInfoVO npcvobyID2 = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
						mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID2.MapCode);
						npcType = 3;
						npcID = npcvobyID2.ID;
					}
				}
				else if (targetType == 7)
				{
					if (xelementAttributeInt != -1)
					{
						NPCInfoVO npcvobyID3 = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
						mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID3.MapCode);
						npcType = 3;
						npcID = npcvobyID3.ID;
					}
				}
				else if (targetType == 8)
				{
					if (xelementAttributeInt != -1)
					{
						MonsterVO monsterXmlNodeByID3 = ConfigMonsters.GetMonsterXmlNodeByID(xelementAttributeInt);
						mapCode = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID3.MapCode);
						npcType = 2;
						npcID = monsterXmlNodeByID3.ID;
					}
				}
				else if (targetType == 9)
				{
					if (xelementAttributeInt != -1)
					{
						NPCInfoVO npcvobyID4 = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
						mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID4.MapCode);
						npcType = 3;
						npcID = npcvobyID4.ID;
					}
				}
				else if (targetType == 10)
				{
					if (xelementAttributeInt != -1)
					{
						NPCInfoVO npcvobyID5 = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
						mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID5.MapCode);
						npcType = 3;
						npcID = npcvobyID5.ID;
					}
				}
				else if (targetType != 4 && xelementAttributeInt != -1)
				{
					NPCInfoVO npcvobyID6 = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
					mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID6.MapCode);
					npcType = 3;
					npcID = npcvobyID6.ID;
				}
			}
			if (posX < 0 && posY < 0)
			{
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(taskXml, "TargetPos" + num);
				if (!string.IsNullOrEmpty(xelementAttributeStr2))
				{
					if (mapCode < 0)
					{
						mapCode = Global.GetXElementAttributeInt(taskXml, "TargetMapCode" + num);
					}
					string[] array2 = xelementAttributeStr2.Split(new char[]
					{
						','
					});
					if (array2.Length == 2)
					{
						posX = array2[0].SafeToInt32(0);
						posY = array2[1].SafeToInt32(0);
					}
				}
			}
			if (replaceFuBen)
			{
				Super.ProcessFuBenNPCOrMonster(taskXml, mapCode, ref isFuBen, ref mapCode, ref npcType, ref npcID);
			}
			return true;
		}

		public static bool GetTaskTargetID(TaskVO taskVO, int num, out int isFuBen, out int mapCode, out int npcType, out int npcID, out int targetType, bool replaceFuBen, out int posX, out int posY)
		{
			isFuBen = 0;
			mapCode = -1;
			npcType = 3;
			npcID = -1;
			posX = -1;
			posY = -1;
			targetType = 0;
			int num2 = 0;
			if (num == 1)
			{
				targetType = taskVO.TargetType1;
				num2 = taskVO.TargetNPC1;
			}
			else if (num == 2)
			{
				targetType = taskVO.TargetType2;
				num2 = taskVO.TargetNPC2;
			}
			else if (num == 0)
			{
				num2 = -1;
			}
			if (targetType == 0)
			{
				if (num2 != -1)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(num2);
					mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID.MapCode);
					npcType = 3;
					npcID = npcvobyID.ID;
				}
				if (Global.IsKuaFuMap(mapCode, false))
				{
					Point npcpointByID = Global.GetNPCPointByID(mapCode, num2);
					posX = npcpointByID.X;
					posY = npcpointByID.Y;
				}
			}
			else if (targetType == 1)
			{
				if (num2 != -1)
				{
					MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(num2);
					mapCode = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID.MapCode);
					npcType = 2;
					npcID = monsterXmlNodeByID.ID;
				}
				if (Global.IsKuaFuMap(mapCode, false))
				{
					Point monsterPointByID = Global.GetMonsterPointByID(mapCode, num2);
					posX = monsterPointByID.X;
					posY = monsterPointByID.Y;
				}
			}
			else if (targetType == 11)
			{
				if (num2 != -1)
				{
					mapCode = taskVO.TargetMapCode1;
					npcType = 11;
					string text = taskVO.TargetPos1.ToString();
					if (!string.IsNullOrEmpty(text))
					{
						string[] array = text.Split(new char[]
						{
							','
						});
						if (array.Length == 2)
						{
							posX = array[0].SafeToInt32(0);
							posY = array[1].SafeToInt32(0);
						}
					}
				}
			}
			else if (targetType == 2)
			{
				if (num2 != -1)
				{
					MonsterVO monsterXmlNodeByID2 = ConfigMonsters.GetMonsterXmlNodeByID(num2);
					mapCode = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID2.MapCode);
					npcType = 2;
					npcID = monsterXmlNodeByID2.ID;
				}
			}
			else if (targetType != 4)
			{
				if (targetType == 5)
				{
					if (num2 != -1)
					{
						NPCInfoVO npcvobyID2 = ConfigNPCs.GetNPCVOByID(num2);
						mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID2.MapCode);
						npcType = 3;
						npcID = npcvobyID2.ID;
					}
				}
				else if (targetType == 7)
				{
					if (num2 != -1)
					{
						NPCInfoVO npcvobyID3 = ConfigNPCs.GetNPCVOByID(num2);
						mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID3.MapCode);
						npcType = 3;
						npcID = npcvobyID3.ID;
					}
				}
				else if (targetType == 8)
				{
					if (num2 != -1)
					{
						if (ShiLiData.BeShiLiTask(taskVO.TaskClass))
						{
							MonsterVO monsterXmlNodeByID3 = ConfigMonsters.GetMonsterXmlNodeByID(num2);
							mapCode = Global.GetNPCOrMonsterMapCodeByID(monsterXmlNodeByID3.MapCode);
							npcType = 2;
							npcID = monsterXmlNodeByID3.ID;
						}
						else
						{
							NPCInfoVO npcvobyID4 = ConfigNPCs.GetNPCVOByID(num2);
							mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID4.MapCode);
							npcType = 3;
							npcID = npcvobyID4.ID;
						}
					}
				}
				else if (targetType == 9)
				{
					if (num2 != -1)
					{
						NPCInfoVO npcvobyID5 = ConfigNPCs.GetNPCVOByID(num2);
						mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID5.MapCode);
						npcType = 3;
						npcID = npcvobyID5.ID;
					}
				}
				else if (targetType == 10)
				{
					if (num2 != -1)
					{
						NPCInfoVO npcvobyID6 = ConfigNPCs.GetNPCVOByID(num2);
						mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID6.MapCode);
						npcType = 3;
						npcID = npcvobyID6.ID;
					}
				}
				else if (199 < targetType && targetType < 228)
				{
					if (num2 != -1)
					{
						NPCInfoVO npcvobyID7 = ConfigNPCs.GetNPCVOByID(num2);
						if (npcvobyID7 != null)
						{
							mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID7.MapCode);
							npcType = 3;
							npcID = npcvobyID7.ID;
						}
					}
				}
				else if (targetType == 13)
				{
					if (num2 != -1)
					{
						NPCInfoVO npcvobyID8 = ConfigNPCs.GetNPCVOByID(num2);
						if (npcvobyID8 != null)
						{
							mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID8.MapCode);
							npcType = 3;
							npcID = npcvobyID8.ID;
						}
					}
				}
				else if (targetType != 4 && num2 != -1)
				{
					NPCInfoVO npcvobyID9 = ConfigNPCs.GetNPCVOByID(num2);
					mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID9.MapCode);
					npcType = 3;
					npcID = npcvobyID9.ID;
				}
			}
			if (posX < 0 && posY < 0)
			{
				string text2 = string.Empty;
				if (num == 1)
				{
					text2 = taskVO.TargetPos1;
				}
				else if (num == 2)
				{
					text2 = taskVO.TargetPos2;
				}
				if (!string.IsNullOrEmpty(text2))
				{
					if (mapCode < 0)
					{
						if (num == 1)
						{
							mapCode = taskVO.TargetMapCode1;
						}
						else if (num == 2)
						{
							mapCode = taskVO.TargetMapCode2;
						}
					}
					string[] array2 = text2.Split(new char[]
					{
						','
					});
					if (array2.Length == 2)
					{
						posX = array2[0].SafeToInt32(0);
						posY = array2[1].SafeToInt32(0);
					}
				}
			}
			if (replaceFuBen)
			{
				Super.ProcessFuBenNPCOrMonster(taskVO, mapCode, ref isFuBen, ref mapCode, ref npcType, ref npcID);
			}
			return true;
		}

		public static string GetTaskTargetNum(XElement taskXml, int doingVal, int num)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(taskXml, StringUtil.substitute("TargetType{0}", new object[]
			{
				num
			}));
			int num2 = Global.GetXElementAttributeInt(taskXml, StringUtil.substitute("TargetNum{0}", new object[]
			{
				num
			}));
			if (num2 <= 0)
			{
				num2 = 1;
			}
			if (xelementAttributeInt != 2 && xelementAttributeInt != 1 && xelementAttributeInt != 8 && xelementAttributeInt != 11 && doingVal > num2)
			{
				doingVal = num2;
			}
			return StringUtil.substitute("({0}/{1})", new object[]
			{
				doingVal,
				num2
			});
		}

		public static string GetTaskTargetNum(TaskVO taskVO, int doingVal, int num)
		{
			int num2 = 0;
			int num3 = 0;
			if (num == 1)
			{
				num2 = taskVO.TargetType1;
				num3 = taskVO.TargetNum1;
			}
			else if (num == 2)
			{
				num2 = taskVO.TargetType2;
				num3 = taskVO.TargetNum2;
			}
			if (num3 <= 0)
			{
				num3 = 1;
			}
			if (num2 != 2 && num2 != 1 && num2 != 8 && num2 != 11 && doingVal > num3)
			{
				doingVal = num3;
			}
			if (num2 == 200)
			{
				int num4 = num3 / 10000;
				int num5 = num3 - num4 * 10000;
				int num6 = doingVal / 10000;
				int num7 = doingVal - num6 * 10000;
				if (num4 >= 2)
				{
					doingVal = num6;
					num3 = num4;
				}
				else
				{
					doingVal = num7;
					num3 = num5;
				}
			}
			return StringUtil.substitute("({0}/{1})", new object[]
			{
				doingVal,
				num3
			});
		}

		public static string GetTaskTargetPos(XElement taskXml, int num)
		{
			string result = string.Empty;
			if (Global.GetXElementAttributeInt(taskXml, StringUtil.substitute("TargetMapCode{0}", new object[]
			{
				num
			})) >= 0)
			{
				result = Global.GetXElementAttributeStr(taskXml, StringUtil.substitute("TargetPos{0}", new object[]
				{
					num
				}));
			}
			return result;
		}

		public static bool JugeTaskGuanLianChengJiu(TaskVO taskVO)
		{
			bool result = true;
			if (Super.IsCurrentTaskGuanLianChengJiu(taskVO))
			{
				result = Super.JugeChengJiuComplete(taskVO);
			}
			return result;
		}

		public static bool IsCurrentTaskGuanLianChengJiu(TaskVO taskVO)
		{
			return taskVO.ChenJiuID > 0;
		}

		public static bool JugeChengJiuComplete(TaskVO data)
		{
			if (data == null)
			{
				return false;
			}
			TaskData taskDataByID = Global.GetTaskDataByID(data.ID);
			if (taskDataByID == null)
			{
				return false;
			}
			if (data.TaskClass != 0)
			{
				if (taskDataByID.ChengJiuVal == -1L)
				{
					return true;
				}
				if (!Super.IsCurrentTaskGuanLianChengJiu(data))
				{
					return true;
				}
			}
			return false;
		}

		public static string EncodingTaskDescMapName(XElement xmlNode)
		{
			return ConfigSettings.GetMapNameByCodeEx(Global.GetNPCOrMonsterMapCodeByID(xmlNode), false);
		}

		public static string EncodingTaskDescMapName(int mapcode)
		{
			return ConfigSettings.GetMapNameByCodeEx(Global.GetNPCOrMonsterMapCodeByID(mapcode), false);
		}

		public static string EncodingTaskDescSName(XElement xmlNode)
		{
			return Global.GetColorStringForNGUIText(new object[]
			{
				"00ff00",
				Global.GetXElementAttributeStr(xmlNode, "SName")
			});
		}

		public static string EncodingTaskDescSName(string sName)
		{
			return Global.GetColorStringForNGUIText(new object[]
			{
				"00ff00",
				sName
			});
		}

		public static string EncodingTaskDescMonsterName(string sName)
		{
			return Global.GetColorStringForNGUIText(new object[]
			{
				"fd010c",
				sName
			});
		}

		public static string GetTaskTargetDesc(XElement taskXml, int num)
		{
			string result = string.Empty;
			int xelementAttributeInt = Global.GetXElementAttributeInt(taskXml, StringUtil.substitute("TargetType{0}", new object[]
			{
				num
			}));
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(taskXml, StringUtil.substitute("TargetNPC{0}", new object[]
			{
				num
			}));
			string xelementAttributeStr = Global.GetXElementAttributeStr(taskXml, "TaskGuild");
			if (xelementAttributeInt == 0)
			{
				if (xelementAttributeInt2 != -1)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt2);
					string text = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
					string text2 = Super.EncodingTaskDescSName(npcvobyID.SName);
					if (!string.IsNullOrEmpty(xelementAttributeStr))
					{
						result = Super.ReplaceConfigContent(xelementAttributeStr, text, text2);
					}
					else
					{
						result = StringUtil.substitute(Global.GetLang("去[{0}]找{1}对话"), new object[]
						{
							text,
							text2
						});
					}
				}
			}
			else if (xelementAttributeInt == 1)
			{
				if (xelementAttributeInt2 != -1)
				{
					MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(xelementAttributeInt2);
					string text3 = Super.EncodingTaskDescMapName(monsterXmlNodeByID.MapCode);
					string text4 = Super.EncodingTaskDescSName(monsterXmlNodeByID.SName);
					if (!string.IsNullOrEmpty(xelementAttributeStr))
					{
						result = Super.ReplaceConfigContent(xelementAttributeStr, text3, text4);
					}
					else
					{
						result = StringUtil.substitute(Global.GetLang("击杀[{0}]{1}"), new object[]
						{
							text3,
							text4
						});
					}
				}
			}
			else if (xelementAttributeInt == 11)
			{
				if (xelementAttributeInt2 != -1)
				{
					result = StringUtil.substitute(Global.GetLang("击杀{0}级以上的怪"), new object[]
					{
						xelementAttributeInt2
					});
				}
			}
			else if (xelementAttributeInt == 2)
			{
				if (xelementAttributeInt2 != -1)
				{
					MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(xelementAttributeInt2);
					string text5 = Super.EncodingTaskDescMapName(monsterXmlNodeByID.MapCode);
					string text6 = Super.EncodingTaskDescSName(monsterXmlNodeByID.SName);
					result = StringUtil.substitute(Global.GetLang("击杀[{0}]{1}获取{2}"), new object[]
					{
						text5,
						text6,
						Super.GetTaskGoodsName(taskXml, num)
					});
				}
			}
			else if (xelementAttributeInt == 3)
			{
				if (xelementAttributeInt2 != -1)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt2);
					string text7 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
					string text8 = Super.EncodingTaskDescSName(npcvobyID.SName);
					result = StringUtil.substitute(Global.GetLang("去[{0}]找{1}购买{2}"), new object[]
					{
						text7,
						text8,
						Super.GetTaskGoodsName(taskXml, num)
					});
				}
				else
				{
					result = StringUtil.substitute(Global.GetLang("购买{0}"), new object[]
					{
						Super.GetTaskGoodsName(taskXml, num)
					});
				}
			}
			else if (xelementAttributeInt == 4)
			{
				string text9 = string.Empty;
				if (Global.GetXElementAttributeInt(taskXml, StringUtil.substitute("TargetMapCode{0}", new object[]
				{
					num
				})) >= 0)
				{
					text9 = Global.GetXElementAttributeStr(taskXml, StringUtil.substitute("TargetPos{0}", new object[]
					{
						num
					}));
				}
				if (!string.IsNullOrEmpty(text9))
				{
					result = StringUtil.substitute(Global.GetLang("去({0})使用{1}"), new object[]
					{
						text9,
						Super.GetTaskGoodsName(taskXml, num)
					});
				}
				else
				{
					result = StringUtil.substitute(Global.GetLang("使用{0}"), new object[]
					{
						Super.GetTaskGoodsName(taskXml, num)
					});
				}
			}
			else if (xelementAttributeInt == 5)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt2);
				string text10 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
				string text11 = Super.EncodingTaskDescSName(npcvobyID.SName);
				result = StringUtil.substitute(Global.GetLang("将{0}交给[{1}]{2}"), new object[]
				{
					Super.GetTaskGoodsName(taskXml, num),
					text10,
					text11
				});
			}
			else if (xelementAttributeInt == 6)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt2);
				result = StringUtil.substitute(Global.GetLang("得到{0}"), new object[]
				{
					Super.GetTaskGoodsName(taskXml, num),
					npcvobyID.SName
				});
			}
			else if (xelementAttributeInt == 7)
			{
				if (xelementAttributeInt2 != -1)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt2);
					string text12 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
					string text13 = Super.EncodingTaskDescSName(npcvobyID.SName);
					result = StringUtil.substitute(Global.GetLang("充值一钻石与[{0}]{1}对话"), new object[]
					{
						text12,
						text13
					});
				}
			}
			else if (xelementAttributeInt == 8)
			{
				if (xelementAttributeInt2 != -1)
				{
					MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(xelementAttributeInt2);
					string text14 = Super.EncodingTaskDescMapName(monsterXmlNodeByID.MapCode);
					string text15 = Super.EncodingTaskDescSName(monsterXmlNodeByID.SName);
					if (!string.IsNullOrEmpty(xelementAttributeStr))
					{
						result = Super.ReplaceConfigContent(xelementAttributeStr, text14, text15);
					}
					else
					{
						result = StringUtil.substitute(Global.GetLang("到[{0}]采集{1}"), new object[]
						{
							text14,
							text15
						});
					}
				}
			}
			else if (xelementAttributeInt == 9)
			{
				if (xelementAttributeInt2 != -1)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt2);
					string text16 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
					string text17 = Super.EncodingTaskDescSName(npcvobyID.SName);
					result = StringUtil.substitute(Global.GetLang("使用{2}治疗[{0}]{1}"), new object[]
					{
						text16,
						text17,
						Super.GetTaskGoodsName(taskXml, num)
					});
				}
			}
			else if (xelementAttributeInt == 10 && xelementAttributeInt2 != -1)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt2);
				string text18 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
				string text19 = Super.EncodingTaskDescSName(npcvobyID.SName);
				result = StringUtil.substitute(Global.GetLang("火烧[{0}]{1}"), new object[]
				{
					text18,
					text19,
					Super.GetTaskGoodsName(taskXml, num)
				});
			}
			return result;
		}

		public static string GetTaskTargetDesc(TaskVO taskVO, int num, bool isShowTaskType = true)
		{
			string text = string.Empty;
			int num2 = 0;
			int num3 = 0;
			if (num == 1)
			{
				num2 = taskVO.TargetType1;
				num3 = taskVO.TargetNPC1;
			}
			else if (num == 2)
			{
				num2 = taskVO.TargetType2;
				num3 = taskVO.TargetNPC2;
			}
			else if (num == 0 && taskVO.TaskClass == 1 && num3 == 0)
			{
				num3 = taskVO.DestNPC;
			}
			string text2 = null;
			if (taskVO.TaskClass == 0)
			{
				text2 = Global.GetLang("[主]");
			}
			else if (taskVO.TaskClass == 1)
			{
				text2 = Global.GetLang("[支]");
			}
			string text3 = string.Empty;
			if (taskVO.TaskClass == 9)
			{
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					"ff37f7",
					text2 + taskVO.Title
				});
			}
			else if (taskVO.TaskClass == 8)
			{
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					"00ff00",
					text2 + taskVO.Title
				});
			}
			else
			{
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					"ffc259",
					text2 + taskVO.Title
				});
			}
			if (num2 == 0)
			{
				if (num3 != -1)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(num3);
					if (npcvobyID != null)
					{
						string text4 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
						string text5 = Super.EncodingTaskDescSName(npcvobyID.SName);
						if (!string.IsNullOrEmpty(taskVO.TaskGuild))
						{
							text = Super.ReplaceConfigContent(taskVO.TaskGuild, text4, text5);
						}
						else
						{
							text = StringUtil.substitute(Global.GetLang("去[{0}]找{1}对话"), new object[]
							{
								text4,
								text5
							});
						}
					}
				}
			}
			else if (num2 == 1)
			{
				if (num3 != -1)
				{
					MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(num3);
					if (monsterXmlNodeByID != null)
					{
						string text6 = Super.EncodingTaskDescMapName(monsterXmlNodeByID.MapCode);
						string text7 = Super.EncodingTaskDescMonsterName(monsterXmlNodeByID.SName);
						if (!string.IsNullOrEmpty(taskVO.TaskGuild))
						{
							text = Super.ReplaceConfigContent(taskVO.TaskGuild, text6, text7);
						}
						else
						{
							text = StringUtil.substitute(Global.GetLang("击杀[{0}]{1}"), new object[]
							{
								text6,
								text7
							});
						}
					}
				}
			}
			else if (num2 == 11)
			{
				if (num3 != -1)
				{
					if (taskVO.ID < 1000)
					{
						text = StringUtil.substitute(Global.GetLang("击杀吊桥怪物"), new object[]
						{
							num3
						});
					}
					else
					{
						text = StringUtil.substitute(Global.GetLang("击杀{0}级以上的怪"), new object[]
						{
							num3
						});
					}
				}
			}
			else if (num2 == 2)
			{
				if (num3 != -1)
				{
					MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(num3);
					string text8 = Super.EncodingTaskDescMapName(monsterXmlNodeByID.MapCode);
					string text9 = Super.EncodingTaskDescMonsterName(monsterXmlNodeByID.SName);
					text = StringUtil.substitute(Global.GetLang("击杀[{0}]{1}获取{2}"), new object[]
					{
						text8,
						text9,
						Super.GetTaskGoodsName(taskVO, num)
					});
				}
			}
			else if (num2 == 3)
			{
				if (num3 != -1)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(num3);
					string text10 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
					string text11 = Super.EncodingTaskDescSName(npcvobyID.SName);
					text = StringUtil.substitute(Global.GetLang("去[{0}]找{1}购买{2}"), new object[]
					{
						text10,
						text11,
						Super.GetTaskGoodsName(taskVO, num)
					});
				}
				else
				{
					text = StringUtil.substitute(Global.GetLang("购买{0}"), new object[]
					{
						Super.GetTaskGoodsName(taskVO, num)
					});
				}
			}
			else if (num2 == 4)
			{
				string text12 = string.Empty;
				if (num == 1)
				{
					if (taskVO.TargetMapCode1 >= 0)
					{
						text12 = taskVO.TargetPos1.ToString();
					}
				}
				else if (num == 2 && taskVO.TargetMapCode2 >= 0)
				{
					text12 = taskVO.TargetPos2.ToString();
				}
				if (!string.IsNullOrEmpty(text12))
				{
					text = StringUtil.substitute(Global.GetLang("去({0})使用{1}"), new object[]
					{
						text12,
						Super.GetTaskGoodsName(taskVO, num)
					});
				}
				else
				{
					text = StringUtil.substitute(Global.GetLang("使用{0}"), new object[]
					{
						Super.GetTaskGoodsName(taskVO, num)
					});
				}
			}
			else if (num2 == 5)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(num3);
				string text13 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
				string text14 = Super.EncodingTaskDescSName(npcvobyID.SName);
				text = StringUtil.substitute(Global.GetLang("将{0}交给[{1}]{2}"), new object[]
				{
					Super.GetTaskGoodsName(taskVO, num),
					text13,
					text14
				});
			}
			else if (num2 == 6)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(num3);
				text = StringUtil.substitute(Global.GetLang("得到{0}"), new object[]
				{
					Super.GetTaskGoodsName(taskVO, num),
					npcvobyID.SName
				});
			}
			else if (num2 == 7)
			{
				if (num3 != -1)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(num3);
					string text15 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
					string text16 = Super.EncodingTaskDescSName(npcvobyID.SName);
					text = StringUtil.substitute(Global.GetLang("充值一钻石与[{0}]{1}对话"), new object[]
					{
						text15,
						text16
					});
				}
			}
			else if (num2 == 8)
			{
				if (num3 != -1)
				{
					if (ShiLiData.BeShiLiTask(taskVO.TaskClass) && taskVO.TargetType1 == 8)
					{
						MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(num3);
						if (monsterXmlNodeByID != null)
						{
							string text17 = Super.EncodingTaskDescMapName(monsterXmlNodeByID.MapCode);
							string text18 = Super.EncodingTaskDescMonsterName(monsterXmlNodeByID.SName);
							if (!string.IsNullOrEmpty(taskVO.TaskGuild))
							{
								text = Super.ReplaceConfigContent(taskVO.TaskGuild, text17, text18);
							}
							else
							{
								text = StringUtil.substitute(Global.GetLang("到[{0}]采集{1}"), new object[]
								{
									text17,
									text18
								});
							}
						}
					}
					else
					{
						NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(num3);
						if (npcvobyID == null)
						{
							text = taskVO.TaskGuild;
						}
						else
						{
							string text19 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
							string text20 = Super.EncodingTaskDescSName(npcvobyID.SName);
							if (!string.IsNullOrEmpty(taskVO.TaskGuild))
							{
								text = Super.ReplaceConfigContent(taskVO.TaskGuild, text19, text20);
							}
							else
							{
								text = StringUtil.substitute(Global.GetLang("到[{0}]采集{1}"), new object[]
								{
									text19,
									text20
								});
							}
						}
					}
				}
			}
			else if (num2 == 9)
			{
				if (num3 != -1)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(num3);
					string text21 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
					string text22 = Super.EncodingTaskDescSName(npcvobyID.SName);
					text = StringUtil.substitute(Global.GetLang("使用{2}治疗[{0}]{1}"), new object[]
					{
						text21,
						text22,
						Super.GetTaskGoodsName(taskVO, num)
					});
				}
			}
			else if (num2 == 10)
			{
				if (num3 != -1)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(num3);
					string text23 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
					string text24 = Super.EncodingTaskDescSName(npcvobyID.SName);
					text = StringUtil.substitute(Global.GetLang("火烧[{0}]{1}"), new object[]
					{
						text23,
						text24,
						Super.GetTaskGoodsName(taskVO, num)
					});
				}
			}
			else if (num2 == 20)
			{
				if (!string.IsNullOrEmpty(taskVO.TaskGuild))
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(num3);
					string text25 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
					string text26 = Super.EncodingTaskDescSName(npcvobyID.SName);
				}
				else
				{
					text = taskVO.TaskGuild;
				}
			}
			else if (199 < num2 && num2 < 228)
			{
				if (!string.IsNullOrEmpty(taskVO.TaskGuild))
				{
					text = taskVO.TaskGuild;
				}
				else
				{
					text = Global.GetLang("TaskGuild为空，策划没有配置描述任务");
				}
			}
			else if (num2 == 13)
			{
				if (!string.IsNullOrEmpty(taskVO.TaskGuild))
				{
					text = taskVO.TaskGuild;
				}
				else
				{
					text = Global.GetLang("TaskGuild为空，策划没有配置描述任务");
				}
			}
			else if (!string.IsNullOrEmpty(taskVO.TaskGuild))
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(num3);
				if (npcvobyID == null)
				{
					text = taskVO.TaskGuild;
				}
				else
				{
					string mapName = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
					string sname = Super.EncodingTaskDescSName(npcvobyID.SName);
					text = Super.ReplaceConfigContent(taskVO.TaskGuild, mapName, sname);
				}
			}
			else
			{
				text = taskVO.TaskGuild;
			}
			if (Super.IsCurrentTaskGuanLianChengJiu(taskVO) && Super.JugeChengJiuHaveProgress(taskVO.ChenJiuID))
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					Super.RedColor,
					string.Format("{0}/{1}", Super.FormatNumString((int)Super.GetChengJiuProgress(taskVO.ID)), Super.FormatNumString(Super.GetCurrentChengJiuSumValue(taskVO.ChenJiuID, 1)))
				});
			}
			if (isShowTaskType)
			{
				return string.Format("{0}\n{1}", text3, text);
			}
			return string.Format("{0}", text);
		}

		public static bool JugeChengJiuHaveProgress(int chengJiuID)
		{
			int currentChengJiuSumValue = Super.GetCurrentChengJiuSumValue(chengJiuID, -1);
			return currentChengJiuSumValue >= 0;
		}

		public static long GetChengJiuProgress(int taskID)
		{
			TaskData taskDataByID = Global.GetTaskDataByID(taskID);
			if (taskDataByID != null)
			{
				return taskDataByID.ChengJiuVal;
			}
			return 0L;
		}

		public static int GetCurrentChengJiuSumValue(int chengJiuID, int defNeedJindu = 1)
		{
			ChengJiuVO chengJiuVOByChengJiuID = ConfigChengJiu.GetChengJiuVOByChengJiuID(chengJiuID);
			int result = defNeedJindu;
			if (chengJiuVOByChengJiuID != null)
			{
				if (chengJiuVOByChengJiuID.ZhuanShengLimit > 0)
				{
					result = chengJiuVOByChengJiuID.ZhuanShengLimit;
				}
				else if (chengJiuVOByChengJiuID.LevelLimit > 0)
				{
					result = chengJiuVOByChengJiuID.LevelLimit;
				}
				else if (chengJiuVOByChengJiuID.LoginDayOne > 0)
				{
					result = chengJiuVOByChengJiuID.LoginDayOne;
				}
				else if (chengJiuVOByChengJiuID.LoginDayTwo > 0)
				{
					result = chengJiuVOByChengJiuID.LoginDayTwo;
				}
				else if (chengJiuVOByChengJiuID.KillMonster > 0)
				{
					result = chengJiuVOByChengJiuID.KillMonster;
				}
				else if (chengJiuVOByChengJiuID.KillBoss > 0)
				{
					result = chengJiuVOByChengJiuID.KillBoss;
				}
				else if (chengJiuVOByChengJiuID.TongQianLimit > 0)
				{
					result = chengJiuVOByChengJiuID.TongQianLimit;
				}
				else if (string.Empty != chengJiuVOByChengJiuID.ZhuiJiaLimit)
				{
					result = 1;
				}
				else if (string.Empty != chengJiuVOByChengJiuID.HeChengLimit)
				{
					result = 1;
				}
				else if (chengJiuVOByChengJiuID.KillRaid > 0)
				{
					result = chengJiuVOByChengJiuID.KillRaid;
				}
				else if (string.Empty != chengJiuVOByChengJiuID.GoodsLimit)
				{
					string[] array = chengJiuVOByChengJiuID.GoodsLimit.Split(new char[]
					{
						','
					});
					result = Global.SafeConvertToInt32(array[0]);
				}
				else if (chengJiuVOByChengJiuID.SkillLevel > 0)
				{
					result = chengJiuVOByChengJiuID.SkillLevel;
				}
			}
			return result;
		}

		public static string FormatNumString(int nValue)
		{
			if (nValue >= 100000000)
			{
				nValue -= nValue % 1000000;
				return string.Format(Global.GetLang("{0}亿"), (int)((double)nValue / 100000000.0));
			}
			if (nValue >= 10000)
			{
				nValue -= nValue % 100;
				return string.Format(Global.GetLang("{0}万"), (int)((double)nValue / 10000.0));
			}
			return nValue.ToString();
		}

		public static string GetTaskInfoPartStr(TaskData taskData, XElement taskXmlNode, int TargetID)
		{
			string text = string.Empty;
			string taskTargetNum = Super.GetTaskTargetNum(taskXmlNode, taskData.DoingTaskVal1, TargetID);
			text = Super.GetTaskTargetDesc(taskXmlNode, TargetID);
			if (text.Length > 0)
			{
				string taskTargetName = Super.GetTaskTargetName(taskXmlNode, TargetID);
				if (string.Empty != taskTargetName)
				{
					int num = -1;
					int num2 = -1;
					int num3 = -1;
					int num4 = -1;
					int num5 = -1;
					int num6;
					int num7;
					Super.GetTaskTargetID(taskXmlNode, TargetID, out num6, out num, out num2, out num3, out num7, false, out num4, out num5);
				}
				if (!string.IsNullOrEmpty(taskTargetNum))
				{
					text = StringUtil.substitute("{0}{1}", new object[]
					{
						text,
						Global.GetColorStringForNGUIText(new object[]
						{
							"fd010c",
							taskTargetNum
						})
					});
				}
			}
			return text;
		}

		public static string GetTaskSourceNPCName(XElement taskXml)
		{
			string result = string.Empty;
			int xelementAttributeInt = Global.GetXElementAttributeInt(taskXml, "SourceNPC");
			if (xelementAttributeInt != -1)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
				result = npcvobyID.SName;
			}
			return result;
		}

		public static string GetTaskSourceNPCName(TaskVO taskVO)
		{
			string result = string.Empty;
			int sourceNPC = taskVO.SourceNPC;
			if (sourceNPC != -1)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(sourceNPC);
				result = npcvobyID.SName;
			}
			return result;
		}

		public static string GetTaskDestNPCName(XElement taskXml)
		{
			string result = string.Empty;
			int xelementAttributeInt = Global.GetXElementAttributeInt(taskXml, "DestNPC");
			if (xelementAttributeInt != -1)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
				result = npcvobyID.SName;
			}
			return result;
		}

		public static string GetTaskDestNPCName(TaskVO taskVO)
		{
			string result = string.Empty;
			int destNPC = taskVO.DestNPC;
			if (destNPC != -1)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(destNPC);
				if (npcvobyID == null)
				{
					return result;
				}
				result = npcvobyID.SName;
			}
			return result;
		}

		public static bool GetTaskSourceNPCID(XElement taskXml, out int mapCode, out int npcType, out int npcID)
		{
			mapCode = -1;
			npcType = 3;
			npcID = -1;
			int xelementAttributeInt = Global.GetXElementAttributeInt(taskXml, "SourceNPC");
			NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
			mapCode = npcvobyID.MapCode;
			npcType = 3;
			npcID = npcvobyID.ID;
			return true;
		}

		public static bool GetTaskSourceNPCID(TaskVO taskVO, out int mapCode, out int npcType, out int npcID)
		{
			mapCode = -1;
			npcType = 3;
			npcID = -1;
			int sourceNPC = taskVO.SourceNPC;
			NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(sourceNPC);
			mapCode = npcvobyID.MapCode;
			npcType = 3;
			npcID = npcvobyID.ID;
			return true;
		}

		public static bool GetTaskDestNPCID(XElement taskXml, ref int mapCode, ref int npcType, ref int npcID)
		{
			mapCode = -1;
			npcType = 3;
			npcID = -1;
			int xelementAttributeInt = Global.GetXElementAttributeInt(taskXml, "DestNPC");
			NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
			mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID.MapCode);
			npcType = 3;
			npcID = npcvobyID.ID;
			return true;
		}

		public static bool GetTaskDestNPCID(TaskVO taskVO, ref int mapCode, ref int npcType, ref int npcID)
		{
			mapCode = -1;
			npcType = 3;
			npcID = -1;
			int destNPC = taskVO.DestNPC;
			NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(destNPC);
			if (npcvobyID != null)
			{
				mapCode = Global.GetNPCOrMonsterMapCodeByID(npcvobyID.MapCode);
				npcType = 3;
				npcID = npcvobyID.ID;
			}
			return true;
		}

		public static string GetTaskSourceNPCDesc(XElement taskXml)
		{
			string result = string.Empty;
			int xelementAttributeInt = Global.GetXElementAttributeInt(taskXml, "SourceNPC");
			if (xelementAttributeInt != -1)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
				string text = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
				string text2 = Super.EncodingTaskDescSName(npcvobyID.SName);
				result = StringUtil.substitute(Global.GetLang("去[{0}]找{1}接取"), new object[]
				{
					text,
					text2
				});
			}
			return result;
		}

		public static string GetTaskSourceNPCDesc(TaskVO taskVO)
		{
			string result = string.Empty;
			int sourceNPC = taskVO.SourceNPC;
			if (sourceNPC != -1)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(sourceNPC);
				if (npcvobyID != null)
				{
					string text = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
					string text2 = Super.EncodingTaskDescSName(npcvobyID.SName);
					result = StringUtil.substitute(Global.GetLang("去[{0}]找{1}接取"), new object[]
					{
						text,
						text2
					});
				}
			}
			return result;
		}

		public static string GetTaskDestNPCDesc(XElement taskXml)
		{
			string result = string.Empty;
			int xelementAttributeInt = Global.GetXElementAttributeInt(taskXml, "DestNPC");
			if (xelementAttributeInt != -1)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
				string text = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
				string text2 = Super.EncodingTaskDescSName(npcvobyID.SName);
				result = StringUtil.substitute(Global.GetLang("去[{0}]找{1}提交"), new object[]
				{
					text,
					text2
				});
			}
			return result;
		}

		public static string GetTaskDestNPCDesc(TaskVO taskVO, bool isShowTaskType = true)
		{
			string text = string.Empty;
			int destNPC = taskVO.DestNPC;
			if (destNPC != -1)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(destNPC);
				string text2 = null;
				if (npcvobyID != null)
				{
					string text3 = Super.EncodingTaskDescMapName(npcvobyID.MapCode);
					string text4 = Super.EncodingTaskDescSName(npcvobyID.SName);
					if (taskVO.TaskClass == 0)
					{
						text2 = Global.GetLang("[主]");
					}
					else if (taskVO.TaskClass == 1)
					{
						text2 = Global.GetLang("[支]");
					}
					string text5 = string.Empty;
					if (taskVO.TaskClass == 9)
					{
						text5 = Global.GetColorStringForNGUIText(new object[]
						{
							"ff37f7",
							text2 + taskVO.Title
						});
					}
					else if (taskVO.TaskClass == 8)
					{
						text5 = Global.GetColorStringForNGUIText(new object[]
						{
							"00ff00",
							text2 + taskVO.Title
						});
					}
					else
					{
						text5 = Global.GetColorStringForNGUIText(new object[]
						{
							"fac60d",
							text2 + taskVO.Title
						});
					}
					if (!string.IsNullOrEmpty(taskVO.TaskTiJiao))
					{
						text = Super.ReplaceConfigContent(taskVO.TaskTiJiao, text3, text4);
					}
					else
					{
						text = StringUtil.substitute(Global.GetLang("去[{0}]找{1}提交"), new object[]
						{
							text3,
							text4
						});
					}
					if (isShowTaskType)
					{
						text = string.Format("{0}\n{1}", text5, text);
					}
				}
				else if (taskVO.TaskClass == 1 && destNPC == 10000)
				{
					text2 = Global.GetLang("[支]");
					text = string.Format("{0}\n{1}", Global.GetColorStringForNGUIText(new object[]
					{
						"fac60d",
						text2 + taskVO.Title
					}), taskVO.TaskTiJiao);
				}
			}
			return text;
		}

		public static bool JugeTaskTargetComplete(XElement xmlNode, int num, int taskVal)
		{
			if (Global.GetXElementAttributeInt(xmlNode, StringUtil.substitute("TargetNPC{0}", new object[]
			{
				num
			})) < 0)
			{
				return true;
			}
			int num2 = Global.GetXElementAttributeInt(xmlNode, StringUtil.substitute("TargetNum{0}", new object[]
			{
				num
			}));
			if (num2 <= 0)
			{
				num2 = 1;
			}
			return taskVal >= num2;
		}

		public static bool JugeTaskTargetComplete(TaskVO taskVO, int num, int taskVal)
		{
			int num2 = 0;
			if (num == 1)
			{
				if (taskVO.TargetType1 == 102 || taskVO.TargetType1 == 101)
				{
					num2 = taskVO.TargetNum1;
				}
				else if (199 < taskVO.TargetType1 && taskVO.TargetType1 < 228)
				{
					num2 = taskVO.TargetNum1;
				}
				else if (taskVO.TargetType1 == 13)
				{
					num2 = taskVO.TargetNum1;
				}
				else
				{
					if (taskVO.TargetNPC1 < 0)
					{
						return true;
					}
					num2 = taskVO.TargetNum1;
				}
			}
			else if (num == 2)
			{
				if (taskVO.TargetType2 == 102 || taskVO.TargetType2 == 101)
				{
					num2 = taskVO.TargetNum2;
				}
				else if (199 < taskVO.TargetType2 && taskVO.TargetType2 < 228)
				{
					num2 = taskVO.TargetNum2;
				}
				else if (taskVO.TargetType2 == 13)
				{
					num2 = taskVO.TargetNum2;
				}
				else
				{
					if (taskVO.TargetNPC2 < 0)
					{
						return true;
					}
					num2 = taskVO.TargetNum2;
				}
			}
			if (num2 <= 0)
			{
				num2 = 1;
			}
			return taskVal >= num2;
		}

		public static bool JugeTaskComplete(TaskVO taskVO, int taskVal1, int taskVal2)
		{
			return Super.JugeTaskTargetComplete(taskVO, 1, taskVal1) && Super.JugeTaskTargetComplete(taskVO, 2, taskVal2);
		}

		public static bool JugeTaskComplete(XElement taskXmlNode, int taskVal1, int taskVal2)
		{
			return Super.JugeTaskTargetComplete(taskXmlNode, 1, taskVal1) && Super.JugeTaskTargetComplete(taskXmlNode, 2, taskVal2);
		}

		public static int GetFocusTaskCount()
		{
			int num = 0;
			if (Global.Data.roleData.TaskDataList == null)
			{
				return num;
			}
			for (int i = 0; i < Global.Data.roleData.TaskDataList.Count; i++)
			{
				if (Global.Data.roleData.TaskDataList[i].DoingTaskFocus > 0)
				{
					num++;
				}
			}
			return num;
		}

		public static string ReplaceConfigContent(string content, string mapName, string SName)
		{
			string text;
			if (content.Contains(Super.args0) && content.Contains(Super.args1))
			{
				text = content.Replace(Super.args0, StringUtil.substitute(Global.GetLang("{0}"), new object[]
				{
					mapName
				}));
				text = text.Replace(Super.args1, StringUtil.substitute(Global.GetLang("{0}"), new object[]
				{
					SName
				}));
			}
			else if (content.Contains(Super.args0))
			{
				text = content.Replace(Super.args0, StringUtil.substitute(Global.GetLang("{0}"), new object[]
				{
					mapName
				}));
			}
			else if (content.Contains(Super.args1))
			{
				text = content.Replace(Super.args1, StringUtil.substitute(Global.GetLang("{0}"), new object[]
				{
					SName
				}));
			}
			else
			{
				text = content;
			}
			return text;
		}

		public static GoodsData GetViewTaskInfoGoodsData(int goodsDbID)
		{
			if (Super.GData.ViewTaskInfoGoodsDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Super.GData.ViewTaskInfoGoodsDataList.Count; i++)
			{
				if (Super.GData.ViewTaskInfoGoodsDataList[i].Id == goodsDbID)
				{
					return Super.GData.ViewTaskInfoGoodsDataList[i];
				}
			}
			return null;
		}

		public static void AutoAcceptTask(int taskID, string npc)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskID);
			if (taskXmlNodeByID == null)
			{
				return;
			}
			int num = 0;
			if (npc == "SourceNPC")
			{
				num = taskXmlNodeByID.SourceNPC;
			}
			else if (npc == "DestNPC")
			{
				num = taskXmlNodeByID.DestNPC;
			}
			if (num <= 0)
			{
				return;
			}
			int npcID = 2130706432 + num;
			GameInstance.Game.SpriteNewTask(npcID, taskID);
		}

		public static List<TalkTextNode> ParseTalkTextInfo(string talkText)
		{
			List<TalkTextNode> list = new List<TalkTextNode>();
			if (string.IsNullOrEmpty(talkText))
			{
				return list;
			}
			bool flag = false;
			int i = 0;
			int num = 0;
			TalkTextNode talkTextNode = null;
			while (i < talkText.Length)
			{
				if (talkText.get_Chars(i) == ',')
				{
					if (!flag && i > num)
					{
						talkTextNode = new TalkTextNode();
						talkTextNode.NpcID = Global.SafeConvertToInt32(talkText.Substring(num, i - num));
						num = i + 1;
					}
				}
				else if (talkText.get_Chars(i) == '｛')
				{
					if (talkTextNode == null)
					{
						talkTextNode = new TalkTextNode();
						talkTextNode.NpcID = Global.SafeConvertToInt32(talkText.Substring(num, i - num));
					}
					flag = true;
					num = i + 1;
				}
				else if (talkText.get_Chars(i) == '｝')
				{
					flag = false;
					if (i > num && talkTextNode != null)
					{
						talkTextNode.TalkText = talkText.Substring(num, i - num);
						list.Add(talkTextNode);
						talkTextNode = null;
						num = i + 1;
					}
				}
				i++;
			}
			return list;
		}

		public static List<TalkTextNode> GetTaskTalkTextInfo(int taskID, string talkName)
		{
			List<TalkTextNode> result = new List<TalkTextNode>();
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskID);
			if (taskXmlNodeByID == null)
			{
				return result;
			}
			string text = string.Empty;
			if (talkName == "AcceptTalk")
			{
				text = taskXmlNodeByID.AcceptTalk;
			}
			else if (talkName == "CompleteTalk")
			{
				text = taskXmlNodeByID.CompleteTalk;
			}
			else if (talkName == "DoingTalk")
			{
				text = taskXmlNodeByID.DoingTalk;
			}
			if (string.IsNullOrEmpty(text))
			{
				return result;
			}
			return Super.ParseTalkTextInfo(text);
		}

		public static string FormatTaskTalkText(List<TalkTextNode> talkList)
		{
			string text = string.Empty;
			for (int i = 0; i < talkList.Count; i++)
			{
				int npcID = talkList[i].NpcID;
				string text2 = (npcID > 0) ? ConfigNPCs.GetNPCNameByID(npcID) : Global.FormatRoleName(Global.Data.roleData);
				string text3 = "00ff00";
				if (text.Length > 0)
				{
					text += "\n\n";
				}
				text += StringUtil.substitute(Global.GetLang("{{{0}}}{1}：{{-}}\r\n{2}"), new object[]
				{
					text3,
					text2,
					talkList[i].TalkText
				});
			}
			return text;
		}

		public static int GetNextMainTask(int taskID)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskID);
			if (taskXmlNodeByID == null)
			{
				return -1;
			}
			if (taskXmlNodeByID.TaskClass != 0)
			{
				return -1;
			}
			int result = -1;
			if (ConfigTasks.TaskXmlNodeDict.Count <= 0)
			{
				return -1;
			}
			foreach (KeyValuePair<int, TaskVO> keyValuePair in ConfigTasks.TaskXmlNodeDict)
			{
				int taskClass = keyValuePair.Value.TaskClass;
				if (taskClass == 0)
				{
					int prevTask = keyValuePair.Value.PrevTask;
					if (prevTask == taskID)
					{
						result = keyValuePair.Value.ID;
						break;
					}
				}
			}
			return result;
		}

		public static string GetTaskTargetGuidDesc(int taskID, int num)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskID);
			if (taskXmlNodeByID == null)
			{
				return string.Empty;
			}
			if (Global.GetTaskDataByID(taskID) == null)
			{
				return string.Empty;
			}
			int num2 = 0;
			int num3 = -1;
			int num4 = 3;
			int num5 = -1;
			int num6 = -1;
			int num7 = -1;
			int num8 = -1;
			if (!Super.GetTaskTargetID(taskXmlNodeByID, num, out num2, out num3, out num4, out num5, out num8, true, out num6, out num7))
			{
				return string.Empty;
			}
			if (num == 1)
			{
				num8 = taskXmlNodeByID.TargetType1;
			}
			else if (num == 2)
			{
				num8 = taskXmlNodeByID.TargetType2;
			}
			if (num8 != 1 && num8 != 2 && num8 != 8)
			{
				return string.Empty;
			}
			int monstreID = 0;
			int num9 = 0;
			string text = string.Empty;
			if (num == 1)
			{
				monstreID = taskXmlNodeByID.TargetNPC1;
				num9 = taskXmlNodeByID.TargetNum1;
				text = taskXmlNodeByID.PropsName1;
			}
			else if (num == 2)
			{
				monstreID = taskXmlNodeByID.TargetNPC2;
				num9 = taskXmlNodeByID.TargetNum2;
				text = taskXmlNodeByID.PropsName2;
			}
			if (taskXmlNodeByID.TaskClass == 1 && num8 == 8)
			{
				return null;
			}
			MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(monstreID);
			if (monsterXmlNodeByID == null)
			{
				return null;
			}
			if (num8 == 1)
			{
				return StringUtil.substitute(Global.GetLang("击杀 {0}{1}/{2}"), new object[]
				{
					monsterXmlNodeByID.SName,
					num,
					num9
				});
			}
			if (num8 == 2)
			{
				return StringUtil.substitute(Global.GetLang("击杀{0} 获取 {1}{2}/{3}"), new object[]
				{
					monsterXmlNodeByID.SName,
					text,
					num,
					num9
				});
			}
			if (num8 == 8)
			{
				return StringUtil.substitute(Global.GetLang("采集 {0}{1}/{2}"), new object[]
				{
					monsterXmlNodeByID.SName,
					num,
					num9
				});
			}
			return string.Empty;
		}

		public static bool JugeTaskNeedEnterFuBen(int taskID)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskID);
			if (taskXmlNodeByID == null)
			{
				return false;
			}
			TaskData taskDataByID = Global.GetTaskDataByID(taskID);
			if (taskDataByID == null)
			{
				return false;
			}
			if (Super.JugeTaskComplete(taskXmlNodeByID, taskDataByID.DoingTaskVal1, taskDataByID.DoingTaskVal2))
			{
				return false;
			}
			int num = 0;
			int mapCode = -1;
			int num2 = 3;
			int num3 = -1;
			int num4 = -1;
			int num5 = -1;
			int num6 = -1;
			if (!Super.JugeTaskTargetComplete(taskXmlNodeByID, 1, taskDataByID.DoingTaskVal1))
			{
				Super.GetTaskTargetID(taskXmlNodeByID, 1, out num, out mapCode, out num2, out num3, out num4, true, out num5, out num6);
			}
			if (Global.GetMapType(mapCode) == MapTypes.NormalCopy)
			{
				return true;
			}
			if (!Super.JugeTaskTargetComplete(taskXmlNodeByID, 2, taskDataByID.DoingTaskVal2))
			{
				Super.GetTaskTargetID(taskXmlNodeByID, 2, out num, out mapCode, out num2, out num3, out num4, true, out num5, out num6);
			}
			return Global.GetMapType(mapCode) != MapTypes.NormalCopy || true;
		}

		public static void CleanUpAllChildWindows(Canvas parent)
		{
			if (null == parent)
			{
				return;
			}
			List<GChildWindow> list = new List<GChildWindow>();
			for (int i = 0; i < parent.Children.numChildren; i++)
			{
			}
			for (int j = 0; j < list.Count; j++)
			{
				list[j].NotifyClose(-1);
			}
		}

		private static QuickKeyItem ParseQuickKeyItem(string item)
		{
			if (string.Empty == StringUtil.trim(item))
			{
				return null;
			}
			if ("undefined@undefined" == StringUtil.trim(item))
			{
				return null;
			}
			string[] array = item.Split(new char[]
			{
				'@'
			});
			if (array.Length != 2)
			{
				return null;
			}
			if (Convert.ToInt32(Global.StringTrim(array[0])) < 0)
			{
				return null;
			}
			int num = Convert.ToInt32(Global.StringTrim(array[1]));
			int itemType = Convert.ToInt32(Global.StringTrim(array[0]));
			if (Global.GetSkillDataByID(num) == null)
			{
				return null;
			}
			return new QuickKeyItem
			{
				ID = num,
				ItemType = itemType
			};
		}

		public static void ParseMainQuickKeys(string keys, bool forceSave = false)
		{
			for (int i = 0; i < Super.GData.MainQuickKeyItems.Length; i++)
			{
				Super.GData.MainQuickKeyItems[i] = null;
			}
			if (keys == null || StringUtil.trim(keys) == string.Empty)
			{
				return;
			}
			string[] array = keys.Split(new char[]
			{
				'|'
			});
			if (array.Length < 4)
			{
				return;
			}
			int num = 0;
			while (num < array.Length && num < Super.GData.MainQuickKeyItems.Length)
			{
				Super.GData.MainQuickKeyItems[num] = Super.ParseQuickKeyItem(array[num]);
				num++;
			}
			string quickKeys = Super.GetQuickKeys(Super.GData.MainQuickKeyItems);
			if (quickKeys != keys || forceSave)
			{
				Global.Data.roleData.MainQuickBarKeys = quickKeys;
				GameInstance.Game.SpriteModKeys(0, quickKeys);
			}
		}

		public static void ParseOtherQuickKeys(string keys)
		{
			for (int i = 0; i < Super.GData.OtherQuickKeyItems.Length; i++)
			{
				Super.GData.OtherQuickKeyItems[i] = null;
			}
			if (keys == null || StringUtil.trim(keys) == string.Empty)
			{
				return;
			}
			string[] array = keys.Split(new char[]
			{
				'|'
			});
			if (array.Length < 4)
			{
				return;
			}
			int num = 0;
			while (num < array.Length && num < Super.GData.MainQuickKeyItems.Length)
			{
				Super.GData.OtherQuickKeyItems[num] = Super.ParseQuickKeyItem(array[num]);
				num++;
			}
		}

		public static string GetQuickKeys(QuickKeyItem[] quickKeyItems)
		{
			if (quickKeyItems == null || quickKeyItems.Length < 4)
			{
				return string.Empty;
			}
			string text = string.Empty;
			for (int i = 0; i < quickKeyItems.Length; i++)
			{
				if (quickKeyItems[i] == null)
				{
					if (text.Length > 0)
					{
						text += "|";
					}
					text += StringUtil.substitute("-1@0", new object[0]);
				}
				else
				{
					if (text.Length > 0)
					{
						text += "|";
					}
					text += StringUtil.substitute("{0}@{1}", new object[]
					{
						quickKeyItems[i].ItemType,
						quickKeyItems[i].ID
					});
				}
			}
			return text;
		}

		public static GIcon GetQuickKeyIcon(QuickKeyItem quickKeyItem, int listBoxType)
		{
			if (quickKeyItem == null)
			{
				return null;
			}
			if (quickKeyItem.ItemType < 0)
			{
				return null;
			}
			string text = null;
			string value;
			int tipType;
			string tip;
			if (quickKeyItem.ItemType == 0)
			{
				SkillData skillDataByID = Global.GetSkillDataByID(quickKeyItem.ID);
				if (skillDataByID == null)
				{
					return null;
				}
				MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(skillDataByID.SkillID);
				if (skillXmlNode == null)
				{
					return null;
				}
				int num = skillXmlNode.MagicIcon;
				if (num < 0)
				{
					num = 0;
				}
				value = Super.GetSkillImageURLFromIconCode(num.ToString(), string.Empty);
				tipType = 2;
				tip = StringUtil.substitute("{0},{1}", new object[]
				{
					skillDataByID.SkillID,
					skillDataByID.SkillLevel
				});
			}
			else
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(quickKeyItem.ID);
				if (goodsXmlNodeByID == null)
				{
					return null;
				}
				value = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
				tipType = 1;
				tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					goodsXmlNodeByID.ID,
					1,
					-1,
					-1
				});
				text = Global.GetTotalGoodsCountByID(quickKeyItem.ID).ToString();
			}
			GIcon gicon = new GIcon(IconTypes.Composite, -1)
			{
				Width = 32.0,
				Height = 32.0,
				TipType = tipType,
				Tip = tip,
				ItemCode = quickKeyItem.ID,
				ItemObject = null,
				BoxTypes = listBoxType,
				TextHorizontalAlignment = global::Layout.Right,
				TextVerticalAlignment = global::Layout.Bottom,
				TextShadowColor = 4278190080U,
				TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 58, 206, 0))
			};
			if (quickKeyItem.ItemType == 0)
			{
				gicon.BodyURL = new ImageURL(value, false, 1);
			}
			else
			{
				gicon.BodyURL = new ImageURL(value, false, 0);
			}
			gicon.Text = text;
			return gicon;
		}

		public static void InitMainQuickKeys()
		{
			if (!string.IsNullOrEmpty(Global.Data.roleData.MainQuickBarKeys))
			{
				string[] array = Global.Data.roleData.MainQuickBarKeys.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (int.Parse(array[i].Split(new char[]
					{
						'@'
					})[0]) != -1)
					{
						return;
					}
				}
			}
			if (Global.Data.roleData.Occupation == 0)
			{
				Global.Data.roleData.MainQuickBarKeys = "-1@0|1@100|-1@0|-1@0";
			}
			else if (Global.Data.roleData.Occupation == 1)
			{
				Global.Data.roleData.MainQuickBarKeys = "-1@0|1@200|-1@0|-1@0";
			}
			else if (Global.Data.roleData.Occupation == 2)
			{
				Global.Data.roleData.MainQuickBarKeys = "-1@0|1@300|-1@0|-1@0";
			}
		}

		public static GoodsData FindOtherRoleGoodsDataByDbID(int dbID)
		{
			if (Super.GData.OtherRoleData == null)
			{
				return null;
			}
			if (Super.GData.OtherRoleData.GoodsDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Super.GData.OtherRoleData.GoodsDataList.Count; i++)
			{
				if (Super.GData.OtherRoleData.GoodsDataList[i].Id == dbID)
				{
					return Super.GData.OtherRoleData.GoodsDataList[i];
				}
			}
			return null;
		}

		public static GoodsData FindOtherRoleGoodsDataByDbID2(int dbID)
		{
			if (Super.GData.OtherRoleData2 == null)
			{
				return null;
			}
			if (Super.GData.OtherRoleData2.GoodsDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Super.GData.OtherRoleData2.GoodsDataList.Count; i++)
			{
				if (Super.GData.OtherRoleData2.GoodsDataList[i].Id == dbID)
				{
					return Super.GData.OtherRoleData2.GoodsDataList[i];
				}
			}
			return null;
		}

		public static void InstantPageRefresh()
		{
			try
			{
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public static GIcon InitPlaySoundControls(Canvas canvas, int x = -1, int y = -1, string tip = null)
		{
			if (x < 0)
			{
			}
			if (y < 0)
			{
			}
			GIcon gicon = new GIcon(IconTypes.HitModes, -1)
			{
				Width = 52.0,
				Height = 44.0,
				Text = "  ",
				TipType = 4,
				Tip = tip,
				TextColor = new SolidColorBrush(uint.MaxValue),
				Cursor = Cursors.Hand
			};
			if (!Global.BackgroundSound.IsPlaying)
			{
				gicon.Hit = true;
			}
			gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				Global.Data.SysSetting.CloseGameMusic = !Global.Data.SysSetting.CloseGameMusic;
				Global.SaveSystemSettings();
				if (Global.Data.SysSetting.CloseGameMusic)
				{
					Global.BackgroundSound.stop();
				}
				else if (Global.Data.roleData != null)
				{
					Global.BackgroundSound.play(Global.WebPath(ConfigSettings.GetMapMusicFileByCode(Global.Data.roleData.MapCode, true)), true);
				}
				e.stopPropagation();
			};
			Canvas.SetLeft(gicon, x + 10);
			Canvas.SetTop(gicon, y + 10);
			Canvas.SetZIndex(gicon, 10.0);
			canvas.Children.Add(gicon);
			return gicon;
		}

		public static GoodsData GetFallGoodsDataByDbID(int id)
		{
			if (Super.GData.CurrentGoodsPackListData == null || Super.GData.CurrentGoodsPackListData.GoodsDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Super.GData.CurrentGoodsPackListData.GoodsDataList.Count; i++)
			{
				if (Super.GData.CurrentGoodsPackListData.GoodsDataList[i].Id == id)
				{
					return Super.GData.CurrentGoodsPackListData.GoodsDataList[i];
				}
			}
			return null;
		}

		public static GoodsData GetExchangeGoodsDataByDbID(int id)
		{
			if (Global.Data.ExchangeDataItem == null)
			{
				return null;
			}
			foreach (KeyValuePair<int, List<GoodsData>> keyValuePair in Global.Data.ExchangeDataItem.GoodsDict)
			{
				List<GoodsData> value = keyValuePair.Value;
				if (value != null)
				{
					for (int i = 0; i < value.Count; i++)
					{
						if (value[i].Id == id)
						{
							return value[i];
						}
					}
				}
			}
			return null;
		}

		public static int GetExhcangeOtherRoleID()
		{
			if (Global.Data.ExchangeDataItem == null)
			{
				return -1;
			}
			return Global.Data.ExchangeDataItem.RequestRoleID;
		}

		public static GoodsData GetStallGoodsDataByDbID(int id)
		{
			if (Global.Data.StallDataItem == null)
			{
				return null;
			}
			if (Global.Data.StallDataItem.GoodsList != null)
			{
				for (int i = 0; i < Global.Data.StallDataItem.GoodsList.Count; i++)
				{
					if (Global.Data.StallDataItem.GoodsList[i].Id == id)
					{
						return Global.Data.StallDataItem.GoodsList[i];
					}
				}
			}
			return null;
		}

		public static GoodsData GetOtherStallGoodsDataByDbID(int id)
		{
			if (Global.Data.OtherStallDataItem == null)
			{
				return null;
			}
			if (Global.Data.OtherStallDataItem.GoodsList != null)
			{
				for (int i = 0; i < Global.Data.OtherStallDataItem.GoodsList.Count; i++)
				{
					if (Global.Data.OtherStallDataItem.GoodsList[i].Id == id)
					{
						return Global.Data.OtherStallDataItem.GoodsList[i];
					}
				}
			}
			return null;
		}

		public static SaleGoodsData GetSaleGoodsDataByDbID(int id)
		{
			if (Super.GData.OtherRolesSaleGoodsDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Super.GData.OtherRolesSaleGoodsDataList.Count; i++)
			{
				if (Super.GData.OtherRolesSaleGoodsDataList[i].GoodsDbID == id)
				{
					return Super.GData.OtherRolesSaleGoodsDataList[i];
				}
			}
			return null;
		}

		public static string GetLineDataText(LineData lineData)
		{
			if (lineData.OnlineCount <= 300)
			{
				return Global.GetLang("顺畅");
			}
			if (lineData.OnlineCount <= 500)
			{
				return Global.GetLang("一般");
			}
			return Global.GetLang("拥挤");
		}

		public static SolidColorBrush GetLineDataBrush(LineData lineData)
		{
			if (lineData.OnlineCount <= 100)
			{
				return new SolidColorBrush(ColorSL.FromArgb(255, 0, 255, 0));
			}
			if (lineData.OnlineCount <= 300)
			{
				return new SolidColorBrush(ColorSL.FromArgb(255, 208, 129, 0));
			}
			return new SolidColorBrush(ColorSL.FromArgb(255, 205, 0, 3));
		}

		public static void HintEndAutoFight(bool showHint = true)
		{
			Global.Data.GameScene.CancelAutoFight(0, true);
			if (showHint)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的操作终止了自动挂机战斗..."), new object[0]), 0, -1, -1, 0);
			}
		}

		public static bool IsDisableUsingGoods()
		{
			return null == Super.GData.PlayZoneRoot && false;
		}

		public static void AddHintGIcon(GIcon icon)
		{
		}

		public static void RemoveHintGIcon(GIcon icon)
		{
		}

		public static void JugeTeamUIItemQueue(GIcon sysIcon)
		{
			Super.JugeTeamUIItemCount += 1.0;
			if (Super.JugeTeamUIItemCount % 20.0 != 0.0)
			{
				return;
			}
			if (Super.GData.TeamUIItemQueue.Count <= 0)
			{
				return;
			}
			if (Global.Data.CurrentTeamData != null && Global.Data.CurrentTeamData.TeamRoles.Count >= 5)
			{
				while (Super.GData.TeamUIItemQueue.Count > 0)
				{
					TeamUIItem teamUIItem = Super.GData.TeamUIItemQueue.shift<TeamUIItem>();
					GameInstance.Game.SpriteTeam(5, teamUIItem.OtherRoleID, 0);
				}
				sysIcon.EnableHint = false;
			}
			else
			{
				TeamUIItem teamUIItem2 = Super.GData.TeamUIItemQueue[0];
				if (Global.GetCorrectLocalTime() - teamUIItem2.AddedTicks >= 30000L)
				{
					GameInstance.Game.SpriteTeam(5, teamUIItem2.OtherRoleID, 0);
					Super.GData.TeamUIItemQueue.shift<TeamUIItem>();
					if (Super.GData.TeamUIItemQueue.Count <= 0)
					{
						sysIcon.EnableHint = false;
					}
				}
			}
		}

		public static bool CanShowFormatedRoleMenu(string text)
		{
			string text2 = Global.GetLang("【") + Global.FormatRoleName(Global.Data.roleData) + Global.GetLang("】");
			if (text == text2)
			{
				return false;
			}
			string[] array = text.Split(new char[]
			{
				'+'
			});
			string title = array[0];
			return ConfigGoods.FindGoodsIDByName(title) == -1 && ConfigMonsters.FindMonsterIDByName(text) == -1 && ConfigNPCs.FindNPCIDByName(text) == -1;
		}

		public static void FormatTextBlockEx(GTextBlockEx textBlockEx)
		{
			string text = textBlockEx.Text;
			List<string> list = new List<string>();
			int num = 0;
			for (int num2 = text.IndexOf('【', num); num2 != -1; num2 = text.IndexOf('【', num))
			{
				int num3 = text.IndexOf('】', num2);
				if (num3 == -1)
				{
					break;
				}
				string source = text.Substring(num2, num3 + 1 - num2);
				list.Add(Global.StringTrim(source));
				num = num3 + 1;
			}
			for (int i = 0; i < list.Count; i++)
			{
				textBlockEx.SetSpecialText(list[i], new SolidColorBrush(4278255360U), true, null, false);
			}
			if (list.Count > 0)
			{
				textBlockEx.RenderText();
			}
		}

		public static SolidColorBrush ParseStringColor(string textColor)
		{
			try
			{
				string text = Global.StringReplaceAll(textColor, "#", string.Empty);
				int a = (int)Convert.ToByte("ff", 16);
				int num = 0;
				if (text.Length == 8)
				{
					a = (int)Convert.ToByte(text.Substring(num, 2), 16);
					num = 2;
				}
				int r = (int)Convert.ToByte(text.Substring(num, 2), 16);
				num += 2;
				int g = (int)Convert.ToByte(text.Substring(num, 2), 16);
				num += 2;
				int b = (int)Convert.ToByte(text.Substring(num, 2), 16);
				uint color = ColorSL.FromArgb(a, r, g, b);
				return new SolidColorBrush(color);
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return new SolidColorBrush(uint.MaxValue);
		}

		public static void ParseFormatTextItem(string textField, out string text, out SolidColorBrush textBrush, out bool underLine, out string tag)
		{
			text = string.Empty;
			textBrush = new SolidColorBrush(uint.MaxValue);
			underLine = false;
			tag = null;
			textField = Global.StringTrim(textField.Substring(1, textField.Length - 1 - 1));
			if (textField.Length <= 4)
			{
				return;
			}
			string[] array = textField.Split(new char[]
			{
				' '
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (Global.StringTrim(array[i]).Length != 0)
				{
					string[] array2 = array[i].Split(new char[]
					{
						'='
					});
					if (array2.Length != 2)
					{
						if (!string.IsNullOrEmpty(text))
						{
							text += " ";
						}
						text += array2[0];
					}
					string text2 = array2[0].ToLower();
					if (text2 == "color")
					{
						string text3 = array2[1].ToLower();
						if (text3.Length == 9 && text3.get_Chars(0) == '#')
						{
							textBrush = Super.ParseStringColor(text3);
						}
					}
					else if (text2 == "uline")
					{
						string text4 = array2[1].ToLower();
						if (text4 == "true")
						{
							underLine = true;
						}
					}
					else if (text2 == "tag")
					{
						string text5 = array2[1];
						if (!string.IsNullOrEmpty(text5))
						{
							tag = text5;
						}
					}
					else if (text2 == "text")
					{
						string text6 = array2[1];
						if (!string.IsNullOrEmpty(text6))
						{
							text = text6;
						}
					}
				}
			}
		}

		public static void FormatTextBlockEx2(GTextBlockEx textBlockEx, string text)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			int num2 = text.IndexOf('｛', num);
			if (num2 == -1)
			{
				stringBuilder.Append(text);
			}
			List<string> list = new List<string>();
			List<SolidColorBrush> list2 = new List<SolidColorBrush>();
			List<bool> list3 = new List<bool>();
			List<string> list4 = new List<string>();
			while (num2 != -1)
			{
				int num3 = text.IndexOf('｝', num2);
				if (num3 == -1)
				{
					stringBuilder.Append(text.Substring(num, text.Length - num));
					break;
				}
				stringBuilder.Append(text.Substring(num, num2 - num));
				string textField = Global.StringTrim(text.Substring(num2, num3 + 1 - num2));
				string text2 = null;
				SolidColorBrush solidColorBrush = null;
				bool flag = false;
				string text3 = null;
				Super.ParseFormatTextItem(textField, out text2, out solidColorBrush, out flag, out text3);
				if (!string.IsNullOrEmpty(text2))
				{
					stringBuilder.Append(text2);
					list.Add(text2);
					list2.Add(solidColorBrush);
					list3.Add(flag);
					list4.Add(text3);
				}
				num = num3 + 1;
				num2 = text.IndexOf('｛', num);
				if (num2 == -1)
				{
					stringBuilder.Append(text.Substring(num, text.Length - num));
				}
			}
			bool flag2 = double.IsNaN(textBlockEx.TextWidth);
			textBlockEx.NoRenderText = stringBuilder.ToString();
			for (int i = 0; i < list.Count; i++)
			{
				textBlockEx.SetSpecialText(list[i], list2[i], list3[i], list4[i], false);
			}
			if (flag2)
			{
				textBlockEx.TextWidth = double.NaN;
			}
			try
			{
				textBlockEx.RenderText();
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		private static int FindSystemNaviTask(int taskID)
		{
			if (Super.SystemNaviDict.Count > 0)
			{
				if (!Super.SystemNaviDict.ContainsKey(taskID))
				{
					return -1;
				}
				return Super.SystemNaviDict[taskID];
			}
			else
			{
				XElement gameResXml = Global.GetGameResXml("Config/SystemNavi.Xml");
				if (gameResXml == null)
				{
					return -1;
				}
				List<XElement> list = null;
				try
				{
					list = Global.GetXElementList(gameResXml, "Item");
				}
				catch (Exception ex)
				{
					MUDebug.LogException(ex);
				}
				if (list == null)
				{
					return -1;
				}
				int num = 0;
				for (int i = 0; i < list.Count; i++)
				{
					XElement xelement = list[i];
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TaskID");
					if (xelementAttributeInt != -1 && !Super.SystemNaviDict.ContainsKey(xelementAttributeInt))
					{
						Super.SystemNaviDict[xelementAttributeInt] = num;
					}
					num++;
				}
				if (Super.SystemNaviDict.Count > 0)
				{
					return Super.FindSystemNaviTask(taskID);
				}
				return -1;
			}
		}

		public static void ClearXMLData()
		{
			if (0 < Super.SystemNaviDict.Count)
			{
				Super.SystemNaviDict.Clear();
			}
		}

		public static SystemNaviBox AddSystemNaviBox(Canvas parentRoot, string partNaviName, string naviText, Point pos, object tag, int decoCode, int delaySecs = -1)
		{
			string name = StringUtil.substitute("SysNavi_{0}", new object[]
			{
				partNaviName
			});
			SystemNaviBox systemNaviBox = U3DUtils.AS<SystemNaviBox>(parentRoot.FindName(name));
			if (null != systemNaviBox)
			{
				systemNaviBox.CenterX = 0.0;
				systemNaviBox.CenterY = 0.0;
				systemNaviBox.Text = naviText;
				systemNaviBox.Coordinate = pos;
				systemNaviBox.DecoCode = decoCode;
				systemNaviBox.Tag = tag;
				return systemNaviBox;
			}
			systemNaviBox = U3DUtils.NEW<SystemNaviBox>();
			systemNaviBox.Name = name;
			systemNaviBox.Text = naviText;
			systemNaviBox.Coordinate = pos;
			systemNaviBox.DecoCode = decoCode;
			Canvas.SetZIndex(systemNaviBox, 9999.0);
			parentRoot.Children.Add(systemNaviBox);
			systemNaviBox.DelayTicks = delaySecs * 1000;
			return systemNaviBox;
		}

		public static bool RemoveSystemNaviBoxByName(Canvas parentRoot, string naviName, object tag)
		{
			SystemNaviBox systemNaviBox = U3DUtils.AS<SystemNaviBox>(parentRoot.FindName(naviName));
			if (null != systemNaviBox)
			{
				parentRoot.Children.Remove(systemNaviBox, true);
				systemNaviBox.DelayTicks = -1;
				systemNaviBox.Destroy();
				return true;
			}
			return false;
		}

		public static bool RemoveSystemNaviBox(Canvas parentRoot, string partNaviName, object tag)
		{
			string naviName = StringUtil.substitute("SysNavi_{0}", new object[]
			{
				partNaviName
			});
			return Super.RemoveSystemNaviBoxByName(parentRoot, naviName, tag);
		}

		private static XElement QuickFindSystemNaviList(int uiType, string uiName, int taskID, int taskStep)
		{
			if (taskID < 0)
			{
				return null;
			}
			int num = Super.FindSystemNaviTask(taskID);
			if (num < 0)
			{
				return null;
			}
			XElement gameResXml = Global.GetGameResXml("Config/SystemNavi.Xml");
			if (gameResXml == null)
			{
				return null;
			}
			List<XElement> list = null;
			try
			{
				list = Global.GetXElementList(gameResXml, "Item");
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			if (list == null)
			{
				return null;
			}
			int num2 = 0;
			for (int i = 0; i < list.Count; i++)
			{
				XElement xelement = list[i];
				if (num2 < num)
				{
					num2++;
				}
				else if (uiName != null && uiName != Global.GetXElementAttributeStr(xelement, "PopupUI"))
				{
					num2++;
				}
				else
				{
					if (uiType == Global.GetXElementAttributeInt(xelement, "UIType") && taskID == Global.GetXElementAttributeInt(xelement, "TaskID") && taskStep == Global.GetXElementAttributeInt(xelement, "TaskStep"))
					{
						return xelement;
					}
					num2++;
				}
			}
			return null;
		}

		public static int GetTaskStateByID(int taskID)
		{
			TaskData taskDataByID = Global.GetTaskDataByID(taskID);
			if (taskDataByID == null)
			{
				return -1;
			}
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskID);
			if (taskXmlNodeByID == null)
			{
				return -1;
			}
			if (Super.JugeTaskComplete(taskXmlNodeByID, taskDataByID.DoingTaskVal1, taskDataByID.DoingTaskVal2))
			{
				return 1;
			}
			return 0;
		}

		public static bool FindHavingMainTask(int taskClass, out TaskData taskData)
		{
			taskData = null;
			List<TaskData> taskDataList = Global.Data.roleData.TaskDataList;
			if (taskDataList == null)
			{
				return false;
			}
			for (int i = 0; i < taskDataList.Count; i++)
			{
				if (taskDataList[i] != null)
				{
					TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskDataList[i].DoingTaskID);
					if (taskXmlNodeByID != null)
					{
						if (taskXmlNodeByID.TaskClass == taskClass)
						{
							taskData = taskDataList[i];
							return true;
						}
					}
				}
			}
			return false;
		}

		public static TaskVO FindNextTask(TaskClasses taskClass)
		{
			TaskVO taskVO;
			if (taskClass == TaskClasses.Main)
			{
				int num = Global.Data.roleData.CompletedMainTaskID;
				if (num <= 0)
				{
					num = (int)ConfigSystemParam.GetSystemParamIntByName("FirstMainTaskID");
				}
				else
				{
					taskVO = ConfigTasks.GetTaskXmlNodeByID(num);
					if (taskVO == null)
					{
						return null;
					}
					num = taskVO.NextTask;
					if (num <= 0)
					{
						return null;
					}
				}
				taskVO = ConfigTasks.GetTaskXmlNodeByID(num);
				if (taskVO == null)
				{
					return null;
				}
			}
			else
			{
				taskVO = ConfigTasks.FindTaskXmlNodeByTaskClass((int)taskClass);
				if (taskVO == null || taskVO.ID <= 0)
				{
					return null;
				}
				int num = taskVO.ID;
			}
			return taskVO;
		}

		public static string GetTaskClassName(int taskClass)
		{
			if (taskClass == 0)
			{
				return Global.GetLang("[主]");
			}
			if (taskClass == 1)
			{
				return Global.GetLang("[支]");
			}
			if (taskClass == 2)
			{
				return Global.GetLang("[循环]");
			}
			if (taskClass == 3)
			{
				return Global.GetLang("[猎杀]");
			}
			if (taskClass == 4)
			{
				return Global.GetLang("[悟性]");
			}
			if (taskClass == 5)
			{
				return Global.GetLang("[军功]");
			}
			if (taskClass == 6)
			{
				return Global.GetLang("[绑定钻石]");
			}
			if (taskClass == 7)
			{
				return Global.GetLang("[帮会]");
			}
			if (taskClass == 8)
			{
				return Global.GetLang("[日常]");
			}
			return Global.GetLang("未知");
		}

		public static SystemNaviBox AddSystemNaviBoxByPos(Canvas parentRoot, string uiName, int taskID, int taskStep, int uiType = 1)
		{
			XElement xelement = Super.QuickFindSystemNaviList(uiType, uiName, taskID, taskStep);
			if (xelement == null)
			{
				return null;
			}
			if (Global.GetXElementAttributeInt(xelement, "HookType") != 0)
			{
				return null;
			}
			Point pos = new Point(Global.GetXElementAttributeInt(xelement, "ToX"), Global.GetXElementAttributeInt(xelement, "ToY"));
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "DelaySecs");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "DecoCode");
			string text = Global.GetXElementAttributeStr(xelement, "UIText");
			if (taskID < 500000)
			{
				string taskTitleByID = Global.GetTaskTitleByID(taskID);
				text = Global.StringReplaceAll(text, "$TASKNAME$", taskTitleByID);
			}
			return Super.AddSystemNaviBox(parentRoot, uiName, text, pos, null, xelementAttributeInt2, xelementAttributeInt);
		}

		private static int GetSystemNaviGoodsID(string goodsText, int occupation)
		{
			if (string.IsNullOrEmpty(goodsText))
			{
				return -1;
			}
			string[] array = goodsText.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2.Length == 2)
				{
					if (!(array2[1] == "-1"))
					{
						if (!(array2[1] == occupation.ToString()))
						{
							goto IL_98;
						}
					}
					try
					{
						return Convert.ToInt32(Global.StringTrim(array2[0]));
					}
					catch (Exception ex)
					{
						MUDebug.LogException(ex);
						break;
					}
				}
				IL_98:;
			}
			return -1;
		}

		public static int AddSystemNaviBoxByGoods(Canvas parentRoot, string uiName, int taskID, int taskStep)
		{
			XElement xelement = Super.QuickFindSystemNaviList(1, uiName, taskID, taskStep);
			if (xelement == null)
			{
				return -1;
			}
			if (Global.GetXElementAttributeInt(xelement, "HookType") != 1)
			{
				return -1;
			}
			int systemNaviGoodsID = Super.GetSystemNaviGoodsID(Global.GetXElementAttributeStr(xelement, "GoodsID"), Global.Data.roleData.Occupation);
			GoodsData goodsDataByID = Global.GetGoodsDataByID(systemNaviGoodsID);
			if (goodsDataByID == null)
			{
				return -1;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "NaviUIType");
			if (xelementAttributeInt > 1)
			{
				return goodsDataByID.Id;
			}
			int goodsDataIndexByDbID = Global.GetGoodsDataIndexByDbID(goodsDataByID.Id, true);
			if (goodsDataIndexByDbID == -1)
			{
				return -1;
			}
			int num = 39;
			int num2 = 38;
			int x = goodsDataIndexByDbID % 6 * num + 18 + 10;
			int y = goodsDataIndexByDbID / 6 * num2 + 17 + 10;
			Point pos = new Point(x, y);
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "DelaySecs");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "DecoCode");
			Super.AddSystemNaviBox(parentRoot, uiName, Global.GetXElementAttributeStr(xelement, "UIText"), pos, (xelementAttributeInt2 != 0) ? null : goodsDataByID.Id, xelementAttributeInt3, xelementAttributeInt2);
			if (xelementAttributeInt == 0)
			{
				return goodsDataByID.Id;
			}
			return 0;
		}

		public static int AddSystemNaviBoxByGoods(Canvas parentRoot, string uiName, GIcon goodsIcon, int indexIcon, string uiText, int decoCode)
		{
			int num = 39;
			int num2 = 38;
			int x = indexIcon % 6 * num + 18 + 10;
			int y = indexIcon / 6 * num2 + 17 + 10;
			Point pos = new Point(x, y);
			Super.AddSystemNaviBox(parentRoot, uiName, uiText, pos, (goodsIcon.ItemObject as GoodsData).Id, decoCode, 0);
			return 0;
		}

		public static void RemoveSystemNaviBoxByGoods(Canvas parentRoot, string partNaviName, int goodsDbID)
		{
			string name = StringUtil.substitute("SysNavi_{0}", new object[]
			{
				partNaviName
			});
			SystemNaviBox systemNaviBox = U3DUtils.AS<SystemNaviBox>(parentRoot.FindName(name));
			if (null != systemNaviBox && systemNaviBox.Tag != null && (int)systemNaviBox.Tag == goodsDbID)
			{
				parentRoot.Children.Remove(systemNaviBox, true);
				systemNaviBox.DelayTicks = -1;
			}
		}

		public static void AddSystemNaviBoxByBtnName(Canvas parentRoot, string uiName, int taskID, int taskStep, int uiType = 1)
		{
			XElement xelement = Super.QuickFindSystemNaviList(uiType, uiName, taskID, taskStep);
			if (xelement == null)
			{
				return;
			}
			if (Global.GetXElementAttributeInt(xelement, "HookType") != 2)
			{
				return;
			}
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "SplashBtn");
			if (string.IsNullOrEmpty(xelementAttributeStr))
			{
				return;
			}
			GIcon gicon = U3DUtils.AS<GIcon>(parentRoot.FindName(xelementAttributeStr));
			if (null == gicon)
			{
				return;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "NaviUIType");
			if (xelementAttributeInt <= 1)
			{
				int num = Math.Max(0, Global.GetXElementAttributeInt(xelement, "ToX"));
				int num2 = Math.Max(0, Global.GetXElementAttributeInt(xelement, "ToY"));
				Point pos = new Point((int)(Canvas.GetLeft(gicon) + (double)num), (int)(Canvas.GetTop(gicon) + (double)num2));
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "DelaySecs");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "DecoCode");
				Super.AddSystemNaviBox(parentRoot, uiName, Global.GetXElementAttributeStr(xelement, "UIText"), pos, (xelementAttributeInt2 != 0) ? null : xelementAttributeStr, xelementAttributeInt3, xelementAttributeInt2);
			}
			if (xelementAttributeInt == 0 || xelementAttributeInt == 2)
			{
				gicon.EnableHint = true;
			}
		}

		public static void AddSystemNaviBoxByBtnName(Canvas parentRoot, string uiName, string uiText, string splashBtnName, int decoCode, int delaySecs = 0)
		{
			if (string.IsNullOrEmpty(splashBtnName))
			{
				return;
			}
			GIcon gicon = U3DUtils.AS<GIcon>(parentRoot.FindName(splashBtnName));
			if (null == gicon)
			{
				return;
			}
			Point pos = new Point((int)(Canvas.GetLeft(gicon) + 10.0), (int)(Canvas.GetTop(gicon) + 10.0));
			Super.AddSystemNaviBox(parentRoot, uiName, uiText, pos, (delaySecs != 0) ? null : splashBtnName, decoCode, delaySecs);
			gicon.EnableHint = true;
		}

		public static void RemoveSystemNaviBoxByBtnName(Canvas parentRoot, string partNaviName, string btnName)
		{
			string name = StringUtil.substitute("SysNavi_{0}", new object[]
			{
				partNaviName
			});
			SystemNaviBox systemNaviBox = U3DUtils.AS<SystemNaviBox>(parentRoot.FindName(name));
			if (null != systemNaviBox && systemNaviBox.Tag != null && (string)systemNaviBox.Tag == btnName)
			{
				parentRoot.Children.Remove(systemNaviBox, true);
				systemNaviBox.DelayTicks = -1;
			}
		}

		public static string GetTaskTriggerPopupUI(int taskID, int taskStep)
		{
			XElement xelement = Super.QuickFindSystemNaviList(2, null, taskID, taskStep);
			if (xelement == null)
			{
				return string.Empty;
			}
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "PopupUI");
			if (string.IsNullOrEmpty(xelementAttributeStr))
			{
				return string.Empty;
			}
			return xelementAttributeStr;
		}

		public static void AddMainMenuUISystemNaviBox(Canvas parentRoot, int taskID, int taskStep)
		{
			if (null != Super.AddSystemNaviBoxByPos(parentRoot, Global.GetLang("主界面菜单UI"), taskID, taskStep, 0))
			{
				return;
			}
			Super.AddSystemNaviBoxByBtnName(parentRoot, Global.GetLang("主界面菜单UI"), taskID, taskStep, 0);
		}

		public static bool CompareTwoEquip(GoodsData newEquip, GoodsData oldEquip)
		{
			if (oldEquip == null)
			{
				return true;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(newEquip.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return false;
			}
			int toLevel = goodsXmlNodeByID.ToLevel;
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (oldEquip == null)
			{
				return true;
			}
			GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(oldEquip.GoodsID);
			if (goodsXmlNodeByID2 == null)
			{
				return false;
			}
			int toLevel2 = goodsXmlNodeByID2.ToLevel;
			return toLevel > toLevel2;
		}

		public static bool CanSaleOutGoods(GoodsData gd)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			return goodsXmlNodeByID.NoSaleOut != 1 && goodsXmlNodeByID.Categoriy != 50;
		}

		public static bool CanDirectSaleOutGoods(GoodsData gd)
		{
			int goodsID = gd.GoodsID;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return false;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy < 0 || categoriy >= 25)
			{
				return false;
			}
			if (gd.Forge_level > 0)
			{
				return false;
			}
			int toLevel = goodsXmlNodeByID.ToLevel;
			if (toLevel > 40)
			{
				return false;
			}
			string text = goodsXmlNodeByID.ToType.ToString();
			return !("-1" != text) || text.Length <= 0;
		}

		public static bool CanHintEquipGoods(int dbID, int goodsID, ref int zhanLiUp)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return false;
			}
			int toSex = goodsXmlNodeByID.ToSex;
			if (toSex != -1 && toSex != Global.Data.roleData.RoleSex)
			{
				return false;
			}
			int toOccupation = goodsXmlNodeByID.ToOccupation;
			if (!Global.ValidOccupation(toOccupation, -1))
			{
				return false;
			}
			int toLevel = goodsXmlNodeByID.ToLevel;
			if (toLevel > Global.Data.roleData.Level)
			{
				return false;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy < 0 || categoriy >= 25)
			{
				return false;
			}
			if (!Global.CanUseGoodsAttr(goodsID, false))
			{
				return false;
			}
			GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(dbID, null);
			if (goodsDataByDbID != null)
			{
				Dictionary<int, int> compareAttributeInfo = Global.GetCompareAttributeInfo(goodsDataByDbID, HandTypes.None);
				if (compareAttributeInfo.ContainsKey(0))
				{
					zhanLiUp = compareAttributeInfo[0];
				}
				else
				{
					zhanLiUp = (int)Global.GetGoodsDataZhanLi(goodsDataByDbID);
				}
			}
			return zhanLiUp > 0;
		}

		public static int HilightsGoodsPacelIcon(Canvas parentRoot, int dbID, int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return -1;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			int toOccupation = goodsXmlNodeByID.ToOccupation;
			int toLevel = goodsXmlNodeByID.ToLevel;
			if ((toOccupation & 1 << Global.Data.roleData.Occupation) == 0 || toLevel > Global.Data.roleData.Level)
			{
				return -1;
			}
			return categoriy;
		}

		public static void HilightsUserAttribIcon(Canvas parentRoot, int goodsID, int decoCode)
		{
			if (Global.Data.roleData.Level >= 10)
			{
				return;
			}
			string goodsNameByID = Global.GetGoodsNameByID(goodsID, false);
			string uiText = StringUtil.substitute(Global.GetLang("您穿上了新装备:{0}, 点击[角色]按钮可以查看属性"), new object[]
			{
				goodsNameByID
			});
			Super.AddSystemNaviBoxByBtnName(parentRoot, Global.GetLang("主界面菜单UI"), uiText, "UserAttribBtn", decoCode, 0);
		}

		public static bool NeedHighlistsGoods(Canvas parentRoot, string uiName, GIcon goodsIcon, int indexIcon, int decoCode)
		{
			GoodsData goodsData = goodsIcon.ItemObject as GoodsData;
			if (goodsData == null)
			{
				return false;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return false;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy < 0 || categoriy >= 25)
			{
				return false;
			}
			int toOccupation = goodsXmlNodeByID.ToOccupation;
			int toLevel = goodsXmlNodeByID.ToLevel;
			if ((toOccupation & 1 << Global.Data.roleData.Occupation) == 0 || toLevel > Global.Data.roleData.Level)
			{
				return false;
			}
			GoodsData goodsDataByType = Global.GetGoodsDataByType(goodsData.Id, goodsData.GoodsID, 1);
			if (goodsDataByType != null)
			{
				return false;
			}
			string goodsNameByID = Global.GetGoodsNameByID(goodsData.GoodsID, false);
			string uiText = StringUtil.substitute(Global.GetLang("鼠标左键双击[{0}]图标装备到身上"), new object[]
			{
				goodsNameByID
			});
			Super.AddSystemNaviBoxByGoods(parentRoot, uiName, goodsIcon, indexIcon, uiText, decoCode);
			return true;
		}

		public static bool IsImportantGoods(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			return goodsXmlNodeByID.Valuables == 1;
		}

		public static uint GetGameInfoTextItemColor(GameInfoTextItem gameInfoTextItem)
		{
			uint result = uint.MaxValue;
			if (gameInfoTextItem.GameInfoTypeIndex != GameInfoTypeIndexes.Normal)
			{
				if (gameInfoTextItem.GameInfoTypeIndex == GameInfoTypeIndexes.Error)
				{
					result = ColorSL.FromArgb(255, 255, 0, 0);
				}
				else if (gameInfoTextItem.GameInfoTypeIndex == GameInfoTypeIndexes.Hot)
				{
					result = 4294967040U;
				}
			}
			return result;
		}

		public static int GetGoodsQuality(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return 0;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy >= 800 && categoriy <= 815)
			{
				if (goodsXmlNodeByID.SuitID >= 3 && goodsXmlNodeByID.SuitID <= 4)
				{
					return 1;
				}
				if (goodsXmlNodeByID.SuitID >= 5 && goodsXmlNodeByID.SuitID <= 6)
				{
					return 2;
				}
				if (goodsXmlNodeByID.SuitID >= 7 && goodsXmlNodeByID.SuitID <= 8)
				{
					return 3;
				}
				if (goodsXmlNodeByID.SuitID >= 9 && goodsXmlNodeByID.SuitID <= 10)
				{
					return 4;
				}
				if (goodsXmlNodeByID.SuitID >= 11)
				{
					return 6;
				}
				return 0;
			}
			else
			{
				if (categoriy == 901)
				{
					int diamondLevelByGoodsID = Global.GetDiamondLevelByGoodsID(goodsID);
					if (diamondLevelByGoodsID >= 2 && diamondLevelByGoodsID <= 3)
					{
						return 1;
					}
					if (diamondLevelByGoodsID >= 4 && diamondLevelByGoodsID <= 5)
					{
						return 2;
					}
					if (diamondLevelByGoodsID >= 6 && diamondLevelByGoodsID <= 7)
					{
						return 3;
					}
					if (diamondLevelByGoodsID >= 8 && diamondLevelByGoodsID <= 10)
					{
						return 4;
					}
				}
				if ((categoriy < 910 || categoriy > 928) && categoriy != 980)
				{
					return goodsXmlNodeByID.ItemQuality;
				}
				if (goodsXmlNodeByID.SuitID == 1)
				{
					return 1;
				}
				if (goodsXmlNodeByID.SuitID == 2)
				{
					return 2;
				}
				if (goodsXmlNodeByID.SuitID == 3)
				{
					return 3;
				}
				if (goodsXmlNodeByID.SuitID >= 4 && goodsXmlNodeByID.SuitID <= 10)
				{
					return 4;
				}
				if (goodsXmlNodeByID.SuitID > 10)
				{
					return 6;
				}
				return 0;
			}
		}

		public static void InitEquipGIcon(GGoodIcon icon, GoodsData goodsData, bool liuguang = true, IconTextTypes iconTextTypes = IconTextTypes.Qianghua)
		{
			int itemCategory = icon.ItemCategory;
			if ((itemCategory >= 0 && itemCategory < 25) || (itemCategory >= 40 && itemCategory <= 45))
			{
				if (iconTextTypes == IconTextTypes.Qianghua)
				{
					int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
					if (null != icon.petLevel)
					{
						if (categoriyByGoodsID == 9 || categoriyByGoodsID == 10)
						{
							if (goodsData.Forge_level >= 0)
							{
								icon.petLevel.text = "Lv" + (goodsData.Forge_level + 1).ToString();
							}
							else
							{
								icon.petLevel.text = string.Empty;
							}
							icon.Text = string.Empty;
						}
						else
						{
							icon.petLevel.text = string.Empty;
							icon.Text = ((goodsData.Forge_level <= 0) ? string.Empty : StringUtil.substitute("+{0}", new object[]
							{
								goodsData.Forge_level.ToString()
							}));
						}
					}
				}
				else if (iconTextTypes == IconTextTypes.Zhuijia)
				{
					icon.Text = ((goodsData.AppendPropLev <= 0) ? string.Empty : StringUtil.substitute(Global.GetLang("追{0}"), new object[]
					{
						goodsData.AppendPropLev.ToString()
					}));
				}
				else if (iconTextTypes == IconTextTypes.Zhuansheng)
				{
					icon.Text = ((goodsData.ChangeLifeLevForEquip <= 0) ? string.Empty : StringUtil.substitute(Global.GetLang("{0}转"), new object[]
					{
						goodsData.ChangeLifeLevForEquip.ToString()
					}));
				}
				icon.TextColor = 4294967040U;
				if (itemCategory == 24 || itemCategory == 8)
				{
					icon.SText = ((goodsData.GCount <= 1) ? string.Empty : goodsData.GCount.ToString());
					icon.Text = ((goodsData.Forge_level <= 0) ? string.Empty : StringUtil.substitute("+{0}", new object[]
					{
						goodsData.Forge_level.ToString()
					}));
				}
				else if (itemCategory == 23)
				{
					icon.SText = ((goodsData.GCount <= 1) ? string.Empty : goodsData.GCount.ToString());
				}
			}
			else if (itemCategory >= 40 && itemCategory <= 45)
			{
				icon.Text = ((goodsData.Forge_level <= 0) ? string.Empty : StringUtil.substitute("+{0}", new object[]
				{
					goodsData.Forge_level.ToString()
				}));
			}
			else if (itemCategory == 980)
			{
				icon.petLevel.text = ((goodsData.ElementhrtsProps[0] <= 0) ? string.Empty : StringUtil.substitute("Lv{0}", new object[]
				{
					goodsData.ElementhrtsProps[0].ToString()
				}));
			}
			else
			{
				icon.SText = ((goodsData.GCount <= 1) ? string.Empty : goodsData.GCount.ToString());
			}
			if (!icon.isAutoSize)
			{
				icon.GoodImg.transform.localScale = new Vector3(64f, 64f, 1f);
				icon.BackgroundSprite1.transform.localScale = new Vector3(62f, 62f, 1f);
				icon.BackgroundSprite2.transform.localScale = new Vector3(64f, 64f, 1f);
			}
		}

		public static UIAtlas GetZhuoYueFlowLightAtlas()
		{
			if (Super.ZhuoYueFlowLightAtlasInstance == null)
			{
				Super.ZhuoYueFlowLightAtlasInstance = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
			}
			return Super.ZhuoYueFlowLightAtlasInstance;
		}

		public static UIAtlas GetZhuoYueFlowLightAtlasCheng()
		{
			if (Super.ZhuoYueFlowLightChengAtlasInstance == null)
			{
				Super.ZhuoYueFlowLightChengAtlasInstance = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLightCheng_bag", true));
			}
			return Super.ZhuoYueFlowLightChengAtlasInstance;
		}

		public static void InitGoodsGIcon(GGoodIcon icon, GoodsData goodsData, bool canUse, IconTextTypes iconTextTypes = IconTextTypes.Qianghua)
		{
			if (icon == null || goodsData == null)
			{
				return;
			}
			Super.InitEquipGIcon(icon, goodsData, true, iconTextTypes);
			if (Global.IsTimeLimitGoods(goodsData))
			{
				icon.EndTimeSprite.gameObject.SetActive(true);
			}
			if (goodsData.Binding > 0)
			{
				icon.BindingSprite.gameObject.SetActive(true);
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return;
			}
			if (goodsXmlNodeByID.Categoriy == 9 || goodsXmlNodeByID.Categoriy == 10)
			{
				if (goodsData.ExcellenceInfo != 0)
				{
					if (icon.TeXiao != null)
					{
						icon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
						icon.TeXiao.gameObject.SetActive(true);
					}
				}
				else if (goodsXmlNodeByID.SuitID == 1)
				{
					icon.BackSpriteName1 = "iconState_zuoyue1";
				}
				else
				{
					icon.BackSpriteName1 = "iconState_zuoyue2";
				}
			}
			else if (goodsXmlNodeByID.Categoriy == 340 || (goodsXmlNodeByID.Categoriy >= 40 && goodsXmlNodeByID.Categoriy <= 45))
			{
				icon.BackSpriteName1 = "none";
				int horseQuality = Super.GetHorseQuality(goodsData);
				if (horseQuality == 1)
				{
					icon.BackSpriteName1 = "iconState_zuoyue";
				}
				else if (horseQuality == 2)
				{
					icon.BackSpriteName1 = "iconState_zuoyue1";
				}
				else if (horseQuality == 3)
				{
					icon.BackSpriteName1 = "iconState_zuoyue2";
				}
				else if (horseQuality == 4 || horseQuality == 5)
				{
					if (icon.TeXiao != null && icon.TeXiao._Sprite != null)
					{
						icon.TeXiao._Sprite.atlas = Super.GetZhuoYueFlowLightAtlas();
						if (!double.IsNaN(icon.Width) && !double.IsNaN(icon.Height))
						{
							icon.TeXiao._Sprite.transform.localScale = new Vector3((float)icon.Width, (float)icon.Height);
						}
						icon.TeXiao.gameObject.SetActive(true);
					}
				}
				else if (horseQuality == 6 && icon.TeXiao._Sprite != null)
				{
					icon.TeXiao._Sprite.atlas = Super.GetZhuoYueFlowLightAtlasCheng();
					if (!double.IsNaN(icon.Width) && !double.IsNaN(icon.Height))
					{
						icon.TeXiao._Sprite.transform.localScale = new Vector3((float)icon.Width, (float)icon.Height);
					}
					icon.TeXiao.gameObject.SetActive(true);
				}
				icon.BackgroundSprite1Visible = true;
			}
			else if (goodsXmlNodeByID.Categoriy == 980)
			{
				int suitID = goodsXmlNodeByID.SuitID;
				if (suitID <= 3)
				{
					icon.BackSpriteName1 = "iconState_zuoyue";
				}
				else if (suitID == 4)
				{
					icon.BackSpriteName1 = "iconState_zuoyue1";
				}
				else if (suitID == 5)
				{
					icon.BackSpriteName1 = "iconState_zuoyue2";
				}
				else if (suitID == 6)
				{
					icon.BackSpriteName1 = "iconState_zuoyue2";
					icon.TeXiao._Sprite.atlas = Super.GetZhuoYueFlowLightAtlas();
					if (!double.IsNaN(icon.Width) && !double.IsNaN(icon.Height))
					{
						icon.TeXiao._Sprite.transform.localScale = new Vector3((float)icon.Width, (float)icon.Height);
					}
					icon.TeXiao.gameObject.SetActive(true);
				}
			}
			else if (Global.GetZhuoyueAttributeCount(goodsData) > 0)
			{
				if (Global.GetZhuoyueAttributeCount(goodsData) >= 6)
				{
					if (icon.TeXiao._Sprite != null)
					{
						icon.TeXiao._Sprite.atlas = Super.GetZhuoYueFlowLightAtlas();
						icon.TeXiao.gameObject.SetActive(true);
					}
				}
				else
				{
					if (Global.GetZhuoyueAttributeCount(goodsData) < 3)
					{
						icon.BackSpriteName1 = "iconState_zuoyue";
					}
					else if (Global.GetZhuoyueAttributeCount(goodsData) >= 3 && Global.GetZhuoyueAttributeCount(goodsData) < 5)
					{
						icon.BackSpriteName1 = "iconState_zuoyue1";
					}
					else if (Global.GetZhuoyueAttributeCount(goodsData) == 5)
					{
						icon.BackSpriteName1 = "iconState_zuoyue2";
					}
					icon.BackgroundSprite1Visible = true;
				}
			}
			else
			{
				int goodsQuality = Super.GetGoodsQuality(goodsData.GoodsID);
				if (goodsQuality == 1)
				{
					icon.BackSpriteName1 = "iconState_zuoyue";
				}
				else if (goodsQuality == 2)
				{
					icon.BackSpriteName1 = "iconState_zuoyue1";
				}
				else if (goodsQuality == 3)
				{
					icon.BackSpriteName1 = "iconState_zuoyue2";
				}
				else if (goodsQuality == 4)
				{
					if (icon.TeXiao._Sprite != null)
					{
						icon.TeXiao._Sprite.atlas = Super.GetZhuoYueFlowLightAtlas();
						if (!double.IsNaN(icon.Width) && !double.IsNaN(icon.Height))
						{
							icon.TeXiao._Sprite.transform.localScale = new Vector3((float)icon.Width, (float)icon.Height);
						}
						icon.TeXiao.gameObject.SetActive(true);
					}
				}
				else if (goodsQuality == 6 && icon.TeXiao._Sprite != null)
				{
					icon.TeXiao._Sprite.atlas = Super.GetZhuoYueFlowLightAtlasCheng();
					if (!double.IsNaN(icon.Width) && !double.IsNaN(icon.Height))
					{
						icon.TeXiao._Sprite.transform.localScale = new Vector3((float)icon.Width, (float)icon.Height);
					}
					icon.TeXiao.gameObject.SetActive(true);
				}
			}
			if (goodsXmlNodeByID.Categoriy == 121)
			{
				icon.ContentText.gameObject.SetActive(false);
				double num = 10000.0;
				double num2 = num * 100.0;
				double num3 = num * 1000.0;
				double num4 = num * 10000.0;
				try
				{
					if (icon.ItemObject != null)
					{
						GoodsData goodsData2 = icon.ItemObject as GoodsData;
						string text = string.Empty;
						string text2 = string.Empty;
						if ((double)goodsData2.GCount < num)
						{
							icon.SText = ((goodsData.GCount <= 1) ? string.Empty : goodsData.GCount.ToString());
						}
						else if ((double)goodsData2.GCount < num3)
						{
							if (num != 0.0)
							{
								text = ((double)goodsData2.GCount / num).ToString();
							}
							text2 = text;
							if (text.Length > 3)
							{
								text2 = text.Substring(0, 3);
							}
							if (text2.EndsWith(".") && text.Length >= 4)
							{
								text2 = text.Substring(0, 4);
							}
							icon.SText = string.Format(Global.GetLang("{0}万"), text2);
						}
						else if ((double)goodsData2.GCount < num4)
						{
							if (num3 != 0.0)
							{
								text = ((double)goodsData2.GCount / num3).ToString();
							}
							text2 = text;
							if (text.Length > 3)
							{
								text2 = text.Substring(0, 3);
							}
							if (text2.EndsWith(".") && text.Length >= 4)
							{
								text2 = text.Substring(0, 4);
							}
							icon.SText = string.Format(Global.GetLang("{0}千万"), text2);
						}
						else
						{
							if (num4 != 0.0)
							{
								text = ((double)goodsData2.GCount / num4).ToString();
							}
							text2 = text;
							if (text.Length > 3)
							{
								text2 = text.Substring(0, 3);
							}
							if (text2.EndsWith(".") && text.Length >= 4)
							{
								text2 = text.Substring(0, 4);
							}
							icon.SText = string.Format(Global.GetLang("{0}亿"), text2);
						}
					}
				}
				catch (Exception ex)
				{
					MUDebug.LogException(ex);
					if (icon.SText.Length == 0)
					{
						icon.SText = ((goodsData.GCount <= 1) ? string.Empty : goodsData.GCount.ToString());
					}
				}
			}
			if (Global.IsShengqi(goodsData))
			{
				if (goodsXmlNodeByID.Categoriy >= 40 && goodsXmlNodeByID.Categoriy <= 45)
				{
					icon.BackSpriteName15 = "horse_iconStateGold";
				}
				else
				{
					icon.BackSpriteName15 = "iconStateGold";
				}
				icon.BackgroundSprite15.transform.localScale = new Vector3(78f, 78f, 1f);
			}
			if (!canUse)
			{
				icon.RefreshIconPos(0);
				icon.NoUseSprite.gameObject.SetActive(true);
			}
			else if (!Global.CanUseGoodsAttr(goodsData.GoodsID, false))
			{
				icon.RefreshIconPos(1);
				icon.NoUseSprite.gameObject.SetActive(true);
			}
			if (null != Super._ParcelPart && Super._ParcelPart.iBaoGuoMode == 8)
			{
				int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
				if (goodsData.Binding > 0)
				{
					icon.NoUseSprite.spriteName = "iconState_nouse3";
					icon.NoUseSprite.gameObject.SetActive(true);
				}
				else if (Global.IsRebornEquip(categoriyByGoodsID))
				{
					if (ChongShengData.beContainBaoShi(goodsData))
					{
						icon.NoUseSprite.spriteName = "iconState_nouse3";
						icon.NoUseSprite.gameObject.SetActive(true);
						Vector3 localPosition = icon.NoUseSprite.transform.localPosition;
						localPosition.z = -0.03f;
						icon.NoUseSprite.transform.localPosition = localPosition;
					}
					else
					{
						icon.NoUseSprite.spriteName = "iconState_nouse3";
						icon.NoUseSprite.gameObject.SetActive(false);
						Vector3 localPosition2 = icon.NoUseSprite.transform.localPosition;
						localPosition2.z = -0.01f;
						icon.NoUseSprite.transform.localPosition = localPosition2;
					}
				}
			}
			if (!icon.isAutoSize)
			{
				icon.GoodImg.transform.localScale = new Vector3(64f, 64f, 1f);
				icon.BackgroundSprite1.transform.localScale = new Vector3(62f, 62f, 1f);
				icon.BackgroundSprite2.transform.localScale = new Vector3(64f, 64f, 1f);
			}
		}

		public static void InitYuansuGoodsGIcon(GGoodIcon icon, GoodsData goodsData)
		{
			if (icon == null || goodsData == null)
			{
				return;
			}
			if (Global.GetCategoriyByGoodsID(goodsData.GoodsID) != 810)
			{
				icon.TextColor = 15990528U;
				icon.Text = string.Format("Lv{0}", Global.GetYuansuGoodsDataLevel(goodsData));
				icon.PaddingX = 10;
				icon.PaddingY = 12;
				icon.TextHorizontalAlignment = global::Layout.Right;
				icon.TextVerticalAlignment = global::Layout.Top;
			}
			else
			{
				icon.Text = string.Empty;
			}
			GoodsQuality goodsQuality = (GoodsQuality)Super.GetGoodsQuality(goodsData.GoodsID);
			if (goodsQuality == GoodsQuality.White)
			{
			}
			if (goodsQuality == GoodsQuality.Green)
			{
				icon.BackSpriteName1 = "iconState_zuoyue";
			}
			else if (goodsQuality == GoodsQuality.Blue)
			{
				icon.BackSpriteName1 = "iconState_zuoyue1";
			}
			else if (goodsQuality == GoodsQuality.Purple)
			{
				icon.BackSpriteName1 = "iconState_zuoyue2";
			}
			else if (goodsQuality == GoodsQuality.FlashPurple)
			{
				if (icon.TeXiao._Sprite != null)
				{
					icon.TeXiao._Sprite.atlas = Super.GetZhuoYueFlowLightAtlas();
					icon.TeXiao._Sprite.transform.localScale = new Vector3((float)icon.Width, (float)icon.Height);
					icon.TeXiao.gameObject.SetActive(true);
				}
			}
			else if (goodsQuality == GoodsQuality.Orange && icon.TeXiao._Sprite != null)
			{
				icon.TeXiao._Sprite.atlas = Super.GetZhuoYueFlowLightAtlasCheng();
				icon.TeXiao._Sprite.transform.localScale = new Vector3((float)icon.Width, (float)icon.Height);
				icon.TeXiao.gameObject.SetActive(true);
			}
			if (!icon.isAutoSize)
			{
				icon.GoodImg.transform.localScale = new Vector3(64f, 64f, 1f);
				icon.BackgroundSprite1.transform.localScale = new Vector3(62f, 62f, 1f);
				icon.BackgroundSprite2.transform.localScale = new Vector3(64f, 64f, 1f);
			}
		}

		public static void SetBgGIconShouStat(GGoodIcon gicon, bool stat)
		{
			if (gicon != null)
			{
				if (stat)
				{
					gicon.BackSpriteName2 = "iconState_sell";
				}
				else
				{
					gicon.BackSpriteName2 = string.Empty;
					gicon.BackgroundSprite2.gameObject.SetActive(false);
				}
			}
		}

		public static void SetSkillIconLiuGuang(GIcon icon, bool liuguang)
		{
			if (liuguang)
			{
				icon.Composite2BodyPath = "Images/Liuguang/0/";
				icon.MaxComposite2BodyCount = 8;
				icon.TimerTicks = 100;
				icon.EnableHint = true;
			}
			else
			{
				icon.Composite2BodyPath = null;
				icon.MaxComposite2BodyCount = 0;
				icon.TimerTicks = 80;
				icon.EnableHint = false;
			}
		}

		public static void CalcRolePropsText(double[] newPropFiles)
		{
			if (Global.Data.CurrentRolePropFields == null)
			{
				return;
			}
			if (Global.Data.CurrentRolePropFields.Length != newPropFiles.Length)
			{
				return;
			}
			for (int i = 0; i < Global.Data.CurrentRolePropFields.Length; i++)
			{
				if (newPropFiles[i] != Global.Data.CurrentRolePropFields[i])
				{
					int num = i - 1;
					if (num == 0)
					{
						double num2 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num2 > 0.0)
						{
						}
					}
					else if (num == 1)
					{
						double num3 = Math.Max(0.0, newPropFiles[i - 1] - Global.Data.CurrentRolePropFields[i - 1]);
						double num4 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num4 > 0.0 || num3 > 0.0)
						{
							ShowTextItem showTextItem = new ShowTextItem
							{
								PicTextColor = 2,
								Text = Global.GetLang("物攻"),
								NumType = ((num4 < 0.0) ? 1 : 0),
								NumVal = num4,
								NumVal2 = (int)Math.Abs(num3),
								NumFormat = 1
							};
							Super.GData.RoleTextQueue.Add(showTextItem);
						}
					}
					else if (num == 2)
					{
						double num5 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num5 > 0.0)
						{
						}
					}
					else if (num == 3)
					{
						double num6 = Math.Max(0.0, newPropFiles[i - 1] - Global.Data.CurrentRolePropFields[i - 1]);
						double num7 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num7 > 0.0 || num6 > 0.0)
						{
							ShowTextItem showTextItem2 = new ShowTextItem
							{
								PicTextColor = 1,
								Text = Global.GetLang("魔攻"),
								NumType = ((num7 < 0.0) ? 1 : 0),
								NumVal = num7,
								NumVal2 = (int)Math.Abs(num6),
								NumFormat = 1
							};
							Super.GData.RoleTextQueue.Add(showTextItem2);
						}
					}
					else if (num == 4)
					{
						double num8 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num8 > 0.0)
						{
						}
					}
					else if (num == 5)
					{
						double num9 = Math.Max(0.0, newPropFiles[i - 1] - Global.Data.CurrentRolePropFields[i - 1]);
						double num10 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num10 > 0.0 || num9 > 0.0)
						{
							ShowTextItem showTextItem3 = new ShowTextItem
							{
								PicTextColor = 3,
								Text = Global.GetLang("物防"),
								NumType = ((num10 < 0.0) ? 1 : 0),
								NumVal = num10,
								NumVal2 = (int)Math.Abs(num9),
								NumFormat = 1
							};
							Super.GData.RoleTextQueue.Add(showTextItem3);
						}
					}
					else if (num == 6)
					{
						double num11 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num11 > 0.0)
						{
						}
					}
					else if (num == 7)
					{
						double num12 = Math.Max(0.0, newPropFiles[i - 1] - Global.Data.CurrentRolePropFields[i - 1]);
						double num13 = (newPropFiles[i + 1] + newPropFiles[i]) / 2.0 - (Global.Data.CurrentRolePropFields[i + 1] + Global.Data.CurrentRolePropFields[i]) / 2.0;
						if (num13 > 0.0 || num12 > 0.0)
						{
							ShowTextItem showTextItem4 = new ShowTextItem
							{
								PicTextColor = 3,
								Text = Global.GetLang("魔防"),
								NumType = ((num13 < 0.0) ? 1 : 0),
								NumVal = num13,
								NumVal2 = (int)Math.Abs(num12),
								NumFormat = 1
							};
							Super.GData.RoleTextQueue.Add(showTextItem4);
						}
					}
					else if (num == 8)
					{
						double num14 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num14 > 0.0)
						{
						}
					}
					else if (num == 9)
					{
						double num15 = Math.Max(0.0, newPropFiles[i - 1] - Global.Data.CurrentRolePropFields[i - 1]);
						double num16 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num16 > 0.0 || num15 > 0.0)
						{
							ShowTextItem showTextItem5 = new ShowTextItem
							{
								PicTextColor = 2,
								Text = Global.GetLang("道攻"),
								NumType = ((num16 < 0.0) ? 1 : 0),
								NumVal = num16,
								NumVal2 = (int)Math.Abs(num15),
								NumFormat = 1
							};
							Super.GData.RoleTextQueue.Add(showTextItem5);
						}
					}
					else if (num == 10)
					{
						double num17 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num17 > 0.0)
						{
						}
					}
					else if (num == 11)
					{
						double num18 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num18 > 0.0)
						{
						}
					}
					else if (num == 12)
					{
						double num19 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num19 > 0.0)
						{
						}
					}
					else if (num == 14)
					{
						double num20 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num20 > 0.0)
						{
						}
					}
					else if (num == 16)
					{
						double num21 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num21 > 0.0)
						{
						}
					}
					else if (num == 17)
					{
						double num22 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num22 > 0.0)
						{
							ShowTextItem showTextItem6 = new ShowTextItem
							{
								PicTextColor = 2,
								Text = Global.GetLang("最大生命值"),
								NumType = ((num22 < 0.0) ? 1 : 0),
								NumVal = (double)((int)Math.Abs(num22))
							};
							Super.GData.RoleTextQueue.Add(showTextItem6);
						}
					}
					else if (num == 19)
					{
						double num23 = newPropFiles[i] - Global.Data.CurrentRolePropFields[i];
						if (num23 > 0.0)
						{
						}
					}
				}
			}
		}

		public static void AddNetImageStream(string key, BitmapData bitmapData)
		{
			Super._NetImagesDict[key] = bitmapData;
		}

		public static BitmapData GetNetImageStream(string key)
		{
			if (!Super._NetImagesDict.ContainsKey(key))
			{
				return null;
			}
			return Super._NetImagesDict[key];
		}

		public static void GetGoods64x64ImageFromFile(string iconCode, GIcon icon)
		{
			try
			{
				Super.DownloadMallNetImage(StringUtil.substitute("NetImages/Sales/{0}.png", new object[]
				{
					iconCode
				}), icon);
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public static void MallDownLoaderComplete1(object sender, Event e)
		{
			Downloader downloader = sender as Downloader;
			Super.mallWaitingDownloaderDict.Remove(downloader.Args);
			downloader.Completed = null;
		}

		public static bool GetMallImageFromCaching(string key, GIcon icon)
		{
			BitmapData netImageStream = Super.GetNetImageStream(key);
			if (netImageStream == null)
			{
				return false;
			}
			if (null != icon)
			{
				icon.BodySource = new ImageBrush(netImageStream);
			}
			else if (Super.mallWaitingDownloaderDict.ContainsKey(key))
			{
				List<GIcon> list = Super.mallWaitingDownloaderDict[key];
				for (int i = 0; i < list.Count; i++)
				{
					list[i].BodySource = new ImageBrush(netImageStream);
				}
			}
			return true;
		}

		public static void DownloadMallNetImage(string value, GIcon icon)
		{
			if (Super.GetMallImageFromCaching(value, icon))
			{
				return;
			}
			icon.BodySource = null;
			List<GIcon> list;
			if (Super.mallWaitingDownloaderDict.ContainsKey(value))
			{
				list = Super.mallWaitingDownloaderDict[value];
				list.Add(icon);
				return;
			}
			list = new List<GIcon>();
			list.Add(icon);
			Super.mallWaitingDownloaderDict[value] = list;
			Downloader downloader = new Downloader(null)
			{
				Args = value
			};
			downloader.GetResourceByVer(Global.WebPath(value), Global.ResSwfVer, false);
		}

		public static bool CanShowSystemWizard(int wizardType)
		{
			return Global.Data.roleData != null && Global.CanMapHelpHint(Global.Data.roleData.MapCode, wizardType) && (!Super._SystemWizardDict.ContainsKey(wizardType) || Super._SystemWizardDict[wizardType]);
		}

		public static void SetShowSystemWizard(int wizardType, bool state)
		{
			Super._SystemWizardDict[wizardType] = state;
		}

		public static BitmapData GetTextBlockImage(GTextBlockOutLine textBlockOutLine)
		{
			return null;
		}

		public static BitmapData GetCanvasImage(Canvas canvas, double width, double height)
		{
			return null;
		}

		public static BitmapData GetSpriteImage(Sprite sprite, double width, double height)
		{
			return null;
		}

		public static void AutoFindRoad(int mapCode, int npcType, int npcID, int buttonID = 1)
		{
			if (Global.Data.WaitingForSystemHelp)
			{
				return;
			}
			if (npcType != -1 && mapCode != -1)
			{
				Global.Data.TargetNpcID = npcID;
				Point pos;
				if (npcType == 2)
				{
					pos = Global.GetMonsterPointByID(mapCode, Global.Data.TargetNpcID);
				}
				else
				{
					pos = Global.GetNPCPointByID(mapCode, Global.Data.TargetNpcID);
				}
				if (pos.X == -1 || pos.Y == -1)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("路径信息格式错误 ,无法自动寻路"), new object[0]), 0, -1, -1, 0);
				}
				else if (npcType == 2)
				{
					Global.Data.GameScene.AutoFindRoad(mapCode, pos, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
					if (buttonID == 0 && Super.CanTransport(mapCode, true, true))
					{
						GameInstance.Game.SpriteTaskTransport(mapCode, pos.X, pos.Y, 1);
					}
				}
				else
				{
					Global.Data.GameScene.AutoFindRoad(mapCode, pos, 120, ExtActionTypes.EXTACTION_NPCDLG);
					if (buttonID == 0 && Super.CanTransport(mapCode, true, true))
					{
						GameInstance.Game.SpriteTaskTransport(mapCode, pos.X, pos.Y, 1);
					}
				}
			}
		}

		public static void PrccessAutoTaskFindRoad(int taskID, bool dontEnterFuBen = false, bool autoTransport = true, bool BAutoFindRode = false, bool BComeFromTTaskBoxMini = false)
		{
			if (!SystemHelpMgr.CanAutoRoad())
			{
				return;
			}
			if (Global.Data.roleData.MapCode == 6090)
			{
				return;
			}
			if (Global.CurrentMapData == null)
			{
				return;
			}
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskID);
			if (taskXmlNodeByID == null)
			{
				return;
			}
			TaskData taskDataByID = Global.GetTaskDataByID(taskID);
			if (taskDataByID == null)
			{
				return;
			}
			int targetType = 0;
			int num = 0;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			int toPosX = -1;
			int toPosY = -1;
			int taskTeleportsByID = Global.GetTaskTeleportsByID(taskID);
			Point point = new Point(0, 0);
			bool flag = false;
			if (!Super.JugeTaskComplete(taskXmlNodeByID, taskDataByID.DoingTaskVal1, taskDataByID.DoingTaskVal2))
			{
				if (!Super.JugeTaskTargetComplete(taskXmlNodeByID, 1, taskDataByID.DoingTaskVal1))
				{
					Super.GetTaskTargetID(taskXmlNodeByID, 1, out num, out num2, out num3, out num4, out targetType, true, out toPosX, out toPosY);
					if (Global.IsGoToKuaFuMap(num2))
					{
						PlayZone.GlobalPlayZone.OpenKuafuMapView(targetType, -1, num4, num2, toPosX, toPosY, false, 0, 0, BAutoFindRode, BComeFromTTaskBoxMini);
						return;
					}
					if (!dontEnterFuBen && num == 1)
					{
						GameInstance.Game.SpriteEnterTaskFuBen(taskID);
						return;
					}
					if (num2 == -1 && taskXmlNodeByID.TargetMapCode1 >= 0)
					{
						targetType = taskXmlNodeByID.TargetType1;
						int targetMapCode = taskXmlNodeByID.TargetMapCode1;
						string text = taskXmlNodeByID.TargetPos1.ToString();
						if (string.Empty != text)
						{
							int[] array = Global.String2IntArray(text, ',');
							if (array != null && array.Length == 2)
							{
								point = new Point(array[0] / Global.CurrentMapData.GridSizeX * Global.CurrentMapData.GridSizeX + Global.CurrentMapData.GridSizeX / 2, array[1] / Global.CurrentMapData.GridSizeY * Global.CurrentMapData.GridSizeY + Global.CurrentMapData.GridSizeY / 2);
								num2 = targetMapCode;
								num3 = -1;
							}
						}
					}
				}
				else
				{
					Super.GetTaskTargetID(taskXmlNodeByID, 2, out num, out num2, out num3, out num4, out targetType, true, out toPosX, out toPosY);
					if (!dontEnterFuBen && num == 1)
					{
						GameInstance.Game.SpriteEnterTaskFuBen(taskID);
						return;
					}
					if (num2 == -1 && taskXmlNodeByID.TargetMapCode2 >= 0)
					{
						targetType = taskXmlNodeByID.TargetType2;
						int targetMapCode2 = taskXmlNodeByID.TargetMapCode2;
						string targetPos = taskXmlNodeByID.TargetPos2;
						if (string.Empty != targetPos)
						{
							int[] array2 = Global.String2IntArray(targetPos, ',');
							if (array2 != null && array2.Length == 2)
							{
								point = new Point(Convert.ToInt32(array2[0]) / Global.CurrentMapData.GridSizeX * Global.CurrentMapData.GridSizeX + Global.CurrentMapData.GridSizeX / 2, Convert.ToInt32(array2[1]) / Global.CurrentMapData.GridSizeY * Global.CurrentMapData.GridSizeY + Global.CurrentMapData.GridSizeY / 2);
								num2 = targetMapCode2;
								num3 = -1;
							}
						}
					}
				}
			}
			else
			{
				flag = true;
				Super.GetTaskDestNPCID(taskXmlNodeByID, ref num2, ref num3, ref num4);
			}
			if (num2 != -1)
			{
				Point pos;
				if (num3 == 2)
				{
					pos = Global.GetMonsterPointByID(num2, num4);
				}
				else if (num3 == 3)
				{
					pos = Global.GetNPCPointByID(num2, num4);
				}
				else
				{
					pos = point;
				}
				if (pos.X == -1 || pos.Y == -1)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("路径信息格式错误 ,无法自动寻路"), new object[0]), 0, -1, -1, 0);
				}
				else
				{
					Global.Data.TargetNpcID = num4;
					if (num3 == 2)
					{
						if (ShiLiData.BeShiLiTask(taskXmlNodeByID.TaskClass) && taskXmlNodeByID.TargetType1 == 8)
						{
							Global.Data.GameScene.AutoFindRoad(num2, pos, 0, ExtActionTypes.EXTACTION_NONE);
							if (0 < taskTeleportsByID && !flag && autoTransport && Super.CanTransport(num2, true, false))
							{
								GameInstance.Game.SpriteTaskTransport2(taskID);
							}
						}
						else
						{
							Global.Data.GameScene.AutoFindRoad(num2, pos, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
							if (0 < taskTeleportsByID && !flag && autoTransport && Super.CanTransport(num2, true, false))
							{
								GameInstance.Game.SpriteTaskTransport2(taskID);
							}
						}
					}
					else if (num3 == 3)
					{
						Global.Data.GameScene.AutoFindRoad(num2, pos, 120, ExtActionTypes.EXTACTION_NPCDLG);
						if (0 < taskTeleportsByID && !flag && autoTransport && Super.CanTransport(num2, true, false))
						{
							GameInstance.Game.SpriteTaskTransport2(taskID);
						}
					}
					else
					{
						Global.Data.GameScene.AutoFindRoad(num2, pos, 0, ExtActionTypes.EXTACTION_NONE);
						if (0 < taskTeleportsByID && !flag && autoTransport && Super.CanTransport(num2, true, true))
						{
							GameInstance.Game.SpriteTaskTransport2(taskID);
						}
					}
				}
			}
		}

		public static void ExternalNavigateURL(string url)
		{
			try
			{
				if (!string.IsNullOrEmpty(url))
				{
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public static void OpenChongZhiHtmlWindow()
		{
			Super.ExternalNavigateURL(Super.GetChongZhiURL());
		}

		public static string GetChongZhiURL()
		{
			return Super.GetJSurl(1);
		}

		public static string GetFangChenMiURL()
		{
			return Super.GetJSurl(5);
		}

		public static void OpenGuanWangHtmlWindow()
		{
			Super.ExternalNavigateURL(Super.GetGuwanWangURL());
		}

		public static string GetGuwanWangURL()
		{
			return Super.GetJSurl(2);
		}

		public static void OpenLunTanHtmlWindow()
		{
			Super.ExternalNavigateURL(Super.GetLunTanURL());
		}

		public static string GetLunTanURL()
		{
			return Super.GetJSurl(3);
		}

		public static void OpenGMHtmlWindow()
		{
			Super.ExternalNavigateURL(Super.GetGMURL());
		}

		public static string GetGMURL()
		{
			return Super.GetJSurl(4);
		}

		public static void AddPersonalHintText(GameInfoTextItem gameInfoTextItem)
		{
			while (Super.GData.PersonalTextItemList.Count >= 100)
			{
				Super.GData.PersonalTextItemList.RemoveRange(0, 1);
			}
			Super.GData.PersonalTextItemList.Add(gameInfoTextItem.TextMsg);
		}

		public static string FormatGoodsOverTime(string dateTime)
		{
			if (string.IsNullOrEmpty(dateTime))
			{
				return string.Empty;
			}
			string[] array = dateTime.Split(new char[]
			{
				' '
			});
			if (array.Length != 2)
			{
				return string.Empty;
			}
			string[] array2 = array[0].Split(new char[]
			{
				'-'
			});
			if (array2.Length != 3)
			{
				return string.Empty;
			}
			string[] array3 = array[1].Split(new char[]
			{
				':'
			});
			if (array3.Length != 3)
			{
				return string.Empty;
			}
			return StringUtil.substitute(Global.GetLang("{0}年{1}月{2}日 {3}时{4}分{5}秒"), new object[]
			{
				array2[0],
				array2[1],
				array2[2],
				array3[0],
				array3[1],
				array3[2]
			});
		}

		public static GoodsData GetYangGongBKGoodsDataByID(int goodsDbID)
		{
			if (Super.GData.YangGongGoodsDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Super.GData.YangGongGoodsDataList.Count; i++)
			{
				if (Super.GData.YangGongGoodsDataList[i].Id == goodsDbID)
				{
					return Super.GData.YangGongGoodsDataList[i];
				}
			}
			return null;
		}

		public static bool CanTransport(int mapCode, bool allowThisMap = false, bool forceTransGoods = true)
		{
			bool flag = Global.Data.GameScene.IsInStalling();
			if (flag)
			{
				return false;
			}
			if (Global.IsAutoFighting())
			{
				Super.HintEndAutoFight(true);
			}
			if (!allowThisMap && Global.Data.roleData.MapCode == mapCode)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("已经在要传送的地图中了"), new object[0]), 0, -1, -1, 0);
				return false;
			}
			if (Global.Data.roleData.MapCode == Global.GetLaoFangMapCode())
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("牢房中无法使用传送功能"), new object[0]), 0, -1, -1, 0);
				return false;
			}
			if (Global.IsBattleMap())
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("阵营战中无法使用传送功能"), new object[0]), 0, -1, -1, 0);
				return false;
			}
			if (Global.Data.roleData.MapCode != 2)
			{
				if (ConfigSystemParam.GetSystemParamIntByName("HuangChengMapCode") == (long)Global.Data.roleData.MapCode)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("皇城地图中无法使用传送功能"), new object[0]), 0, -1, -1, 0);
					return false;
				}
				if (Global.GetLingDiIDByMapCode2(Global.Data.roleData.MapCode) > 0)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领地地图中无法使用传送功能"), new object[0]), 0, -1, -1, 0);
					return false;
				}
			}
			return !forceTransGoods || Global.ProcessMonthVIP() > 0.0 || Global.GetGoodsDataByID(Global.MapTransGoodsID) != null || Global.GetGoodsDataByID(Global.MapTransGoodsID2) != null;
		}

		public static BitmapData ConvertBitmapDataByGrid9(BitmapData bmd, double newWidth, double newHeight, double offsetX, double offsetY)
		{
			return null;
		}

		public static GoodsData GetShowGoodsDataBy(int goodsDbID, int goodsOwner, int goodsID)
		{
			GoodsData goodsData = null;
			if (goodsDbID == -1)
			{
				goodsData = null;
			}
			else if (goodsOwner == 0 || goodsOwner == 24)
			{
				goodsData = Global.GetGoodsDataByDbID(goodsDbID, null);
			}
			else if (goodsOwner == 1)
			{
				goodsData = Super.FindOtherRoleGoodsDataByDbID(goodsDbID);
			}
			else if (goodsOwner == 2)
			{
				goodsData = Super.GetFallGoodsDataByDbID(goodsDbID);
			}
			else if (goodsOwner == 3)
			{
				goodsData = Super.GetExchangeGoodsDataByDbID(goodsDbID);
			}
			else if (goodsOwner == 4)
			{
				goodsData = Super.GetStallGoodsDataByDbID(goodsDbID);
			}
			else if (goodsOwner == 5)
			{
				goodsData = Super.GetOtherStallGoodsDataByDbID(goodsDbID);
			}
			else if (goodsOwner == 6)
			{
				goodsData = Global.GetPortableGoodsDataByDbID(goodsDbID, false);
			}
			else if (goodsOwner == 7)
			{
				goodsData = Global.GetPackGoodsDataByDbID(goodsDbID);
			}
			else if (goodsOwner == 8)
			{
				goodsData = Global.GetSaleGoodsDataByDbID(goodsDbID);
			}
			else if (goodsOwner == 9)
			{
				goodsData = Global.GetOtherSaleGoodsDataByDbID(goodsDbID);
			}
			else if (goodsOwner == 10)
			{
				SaleGoodsData saleGoodsDataByDbID = Super.GetSaleGoodsDataByDbID(goodsDbID);
				if (saleGoodsDataByDbID != null)
				{
					goodsData = saleGoodsDataByDbID.SalingGoodsData;
				}
			}
			else if (goodsOwner == 11)
			{
				goodsData = Global.Data.WaBaoGoodsData;
			}
			else if (goodsOwner == 12)
			{
				goodsData = Global.GetGiftGoodsDataByID(goodsDbID);
			}
			else if (goodsOwner == 13)
			{
				goodsData = Super.GData.UpgradeEquipGoodsData;
			}
			else if (goodsOwner == 14)
			{
				goodsData = Super.FindOtherRoleGoodsDataByDbID2(goodsDbID);
			}
			else if (goodsOwner == 15)
			{
				goodsData = Super.GetViewTaskInfoGoodsData(goodsDbID);
			}
			else if (goodsOwner == 16)
			{
				goodsData = Super.GetYangGongBKGoodsDataByID(goodsDbID);
			}
			else if (goodsOwner == 17)
			{
				goodsData = Super.GData.QuickEnchanceEquipGoodsData;
			}
			else if (goodsOwner == 18)
			{
				goodsData = Super.GData.QuickForgeEquipGoodsData;
			}
			else if (goodsOwner == 19)
			{
				goodsData = Super.GData.CurrentChatGoodsData;
			}
			else if (goodsOwner == 20)
			{
				goodsData = Global.GetEmailGoodsDataByID(goodsDbID);
			}
			else if (goodsOwner == 21)
			{
				goodsData = Global.GetNpcSaleGoodsDataByID(goodsDbID);
			}
			else if (goodsOwner == 22)
			{
				goodsData = Global.GetBaoKuJiangLiGoodsDataByID(goodsDbID);
			}
			if (goodsData == null)
			{
				goodsData = Global.GetDummyGoodsDataMu(goodsID, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			}
			return goodsData;
		}

		public static void UseSkillByID(int skillID)
		{
			if (ConfigMagicInfos.CanSkillByBangDing(skillID, false))
			{
				Global.Data.GameScene.SetDefaultSkillID(skillID);
			}
			if (Global.CanMapUseMagic(Global.CurrentMapData, skillID))
			{
				if (ConfigMagicInfos.GetSkillQueueTicks(skillID) <= 0L)
				{
					if (!Global.SkillCoolDown(skillID))
					{
						if (!Global.IsAutoFighting())
						{
							if (Global.Data.GameScene.CanDoMagicAttackNow())
							{
								if (ConfigMagicInfos.GetSkillActionType(skillID) == 0)
								{
									Global.Data.GameScene.DoAttack(false);
								}
								else
								{
									Global.Data.GameScene.DoMagicAttack(skillID, new Point(-1, -1), null, false, true);
								}
							}
						}
						else
						{
							Global.Data.GameScene.DoMagicAttack(skillID, new Point(-1, -1), null, true, true);
						}
					}
					else
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang(" [{0}]技能仍在CD中，请稍后再使用"), new object[]
						{
							ConfigMagicInfos.GetSkillNameByID(skillID)
						}), 0, -1, -1, 0);
					}
				}
				else if (!Global.SkillCoolDown(skillID))
				{
					Global.Data.GameScene.SetTempWaitingSkillID(skillID);
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang(" [{0}]技能仍在CD中，请稍后再使用"), new object[]
					{
						ConfigMagicInfos.GetSkillNameByID(skillID)
					}), 0, -1, -1, 0);
				}
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("本地图禁止使用[{0}]技能"), new object[]
				{
					ConfigMagicInfos.GetSkillNameByID(skillID)
				}), 0, -1, -1, 0);
			}
		}

		public static string GetJSurl(int index)
		{
			return string.Empty;
		}

		public static Point GetObjectCenterPos(GameObject go)
		{
			return new Point(0, 0);
		}

		public static void SetLeadTargetPos(int mapCode, Vector3 pos)
		{
			if (pos.x >= 0f && pos.z > 0f)
			{
				Super.GData.LeadInfo.MapCode = mapCode;
				Super.GData.LeadInfo.TaskTargetPos = pos;
			}
			else
			{
				Super.GData.LeadInfo.MapCode = -1;
			}
		}

		public static void InitDonwloadConfig()
		{
			GameObject gameObject = new GameObject("LoadConfig");
			gameObject.AddComponent<LoadConfig>();
		}

		public static void InitRootParams()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["uid"] = "-1";
			if (Application.isEditor || Application.platform == 2)
			{
				string ipAddress = Network.player.ipAddress;
				if (ipAddress == "192.168.0.66" || ipAddress == "192.168.0.145" || ipAddress == "192.168.1.248" || ipAddress == "192.168.1.166")
				{
					dictionary["serverip"] = "192.168.0.145";
				}
				else
				{
					dictionary["serverip"] = "192.168.0.206";
					dictionary["voiceserverip"] = "192.168.0.205:8080/test/audiochat/";
					dictionary["pushserverip"] = "192.168.0.205:8080/test/audiochat/";
					dictionary["adserverip"] = "192.168.0.205:8080/test/ad/";
					dictionary["serverlisturl"] = "192.168.0.205:8080/UCLogin/GetServerList.aspx";
					dictionary["serverlisturlsecond"] = "192.168.0.205:8080/UCLogin/GetServerList.aspx";
					dictionary["serverlistCPUrl"] = "192.168.0.205:8080/UCLogin/GetServerList.aspx";
					dictionary["verifyaccountserverip"] = "localhost:1131/";
					dictionary["payserverip"] = "localhost:1131/";
					dictionary["login"] = "0";
					dictionary["gameid"] = "local";
					dictionary["lang"] = "0";
					dictionary["isolateresid"] = "1";
					dictionary["roleNum"] = "0";
					dictionary["deleteRole"] = "1";
					dictionary["loginport"] = "4402";
					dictionary["gameport"] = "4403";
				}
			}
			else
			{
				dictionary["serverip"] = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "serverip");
				dictionary["adserverip"] = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "adserverip");
				dictionary["voiceserverip"] = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "voiceserverip");
				dictionary["pushserverip"] = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "pushserverip");
				dictionary["serverlisturl"] = Global.ReadXmlConfigStr((!Context.IsAPPVerifyC) ? Global.NetVersionXML : Global.GetXmlFromResource("version_IosVerify.xml"), "Info", "serverlisturl");
				dictionary["serverlisturlsecond"] = Global.ReadXmlConfigStr((!Context.IsAPPVerifyC) ? Global.NetVersionXML : Global.GetXmlFromResource("version_IosVerify.xml"), "Info", "serverlisturlsecond");
				dictionary["verifyaccountserverip"] = Global.ReadXmlConfigStr((!Context.IsAPPVerifyC) ? Global.NetVersionXML : Global.GetXmlFromResource("version_IosVerify.xml"), "Info", "verifyaccountserverip");
				dictionary["payserverip"] = Global.ReadXmlConfigStr((!Context.IsAPPVerifyC) ? Global.NetVersionXML : Global.GetXmlFromResource("version_IosVerify.xml"), "Info", "payserverip");
				dictionary["serverid"] = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "serverid");
				dictionary["gameid"] = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "gameid");
				dictionary["login"] = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "login");
				dictionary["lang"] = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "lang");
				dictionary["isolateresid"] = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "isolateresid");
				dictionary["roleNum"] = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "roleNum");
				dictionary["deleteRole"] = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "deleteRole");
				dictionary["loginport"] = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "loginport");
				dictionary["gameport"] = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "gameport");
				dictionary["resver"] = Global.ReadXmlConfigStr(Global.VersionXml, "Resource", "VerCode");
				dictionary["resverText"] = Global.ReadXmlConfigStr(Global.VersionXml, "Resource", "VerText");
				dictionary["exever"] = Global.ReadXmlConfigStr(Global.VersionXml, "Application", "VerCode");
				dictionary["exeverText"] = Global.ReadXmlConfigStr(Global.VersionXml, "Application", "VerText");
			}
			Global.RootParams = dictionary;
		}

		public static void InitParams()
		{
			string xapParamByName = Super.GetXapParamByName("country", string.Empty);
			if ("korea" == xapParamByName)
			{
				MyDateTime.Before1970Ticks += 36000000000L;
			}
			else if ("vietnam" == xapParamByName)
			{
				MyDateTime.Before1970Ticks -= 36000000000L;
			}
			Context.GameLang = Global.SafeConvertToInt32(Super.GetXapParamByName("lang", "0"));
			Context.IsolateResID = Super.GetXapParamByName("isolateresid", "1");
			Context.PingTaiName = Super.GetXapParamByName("pingtainame", "local");
			ProtocolTypes.EnableTengXunTGW = ("TengXun" == Global.PingTaiName);
			Context.XapAbsoluteWebPath = Super.GetXapParamByName("XapWebPath", string.Empty);
			Context.ReportStatURL = Super.GetXapParamByName("serverip", string.Empty);
			Global.ChatVoiceServerURL = Super.GetXapParamByName("voiceserverip", string.Empty);
			Global.VerifyAccountServerURL = Super.GetXapParamByName("verifyaccountserverip", string.Empty);
			Global.PayServerURL = Super.GetXapParamByName("payserverip", string.Empty);
			Global.ServerListURL = Super.GetXapParamByName("serverlisturl", string.Empty);
			Global.ServerListURLSecond = Super.GetXapParamByName("serverlisturlsecond", string.Empty);
			Global.ServerListCrossPlatfomURL = Super.GetXapParamByName("serverlistCPUrl", string.Empty);
			Global.PushServerURL = Super.GetXapParamByName("pushserverip", string.Empty);
			Global.AdServerUrl = Super.GetXapParamByName("adserverip", string.Empty);
			Global.AppealURL = Global.VerifyAccountServerURL + "Appeal.aspx";
		}

		public static void InitVersions()
		{
		}

		public static void PlaySelectRoleSound(GameObject go, string fileName, float delaySecs = 0f)
		{
			string url = StringUtil.substitute("Audio/RoleSelect/{0}", new object[]
			{
				fileName
			});
			NetAudioSource netAudioSource = go.GetComponent<NetAudioSource>();
			if (null == netAudioSource)
			{
				netAudioSource = go.AddComponent<NetAudioSource>();
				netAudioSource.DelayPlaySecs = delaySecs;
				netAudioSource.PlayAudio(url, false, false);
			}
			else
			{
				netAudioSource.PlayAgain();
			}
		}

		public static void PlayGoodsSound(int goodsID)
		{
			GameObject gameObject = GameObject.Find("MyNetAudioSource");
			if (null == gameObject)
			{
				gameObject = new GameObject("MyNetAudioSource", new Type[]
				{
					typeof(AudioSource)
				});
			}
			NetAudioSource netAudioSource = gameObject.GetComponent<NetAudioSource>();
			if (null == netAudioSource)
			{
				netAudioSource = gameObject.AddComponent<NetAudioSource>();
				gameObject.GetComponent<AudioSource>().spatialBlend = 0f;
			}
			string goodsGetSoundByID = Global.GetGoodsGetSoundByID(goodsID);
			if (!string.IsNullOrEmpty(goodsGetSoundByID))
			{
				if (Global.Data.SysSetting.CloseGameAudio)
				{
					netAudioSource.StopPlay();
				}
				else
				{
					netAudioSource.PlayAudio(string.Format("Audio/Goods/{0}", goodsGetSoundByID), false, false);
				}
			}
		}

		public static void PlayYinDaoSound(string musicName, bool force = true, bool loop = false)
		{
			if (Global.Data.SysSetting.CloseGameAudio)
			{
				return;
			}
			GameObject gameObject = GameObject.Find("YinDaoNetAudioSource");
			if (null == gameObject)
			{
				gameObject = new GameObject("YinDaoNetAudioSource", new Type[]
				{
					typeof(AudioSource)
				});
			}
			NetAudioSource netAudioSource = gameObject.GetComponent<NetAudioSource>();
			if (null == netAudioSource)
			{
				netAudioSource = gameObject.AddComponent<NetAudioSource>();
				gameObject.GetComponent<AudioSource>().spatialBlend = 0f;
			}
			netAudioSource.PlayAudio(string.Format("Audio/YinDao/{0}", musicName), loop, force);
		}

		public static GChildWindow ShowGuideWindow(BaodianGuidePart.GuideType errType, DPSelectedItemEventHandler callback, string customLinkIDs = "", string customTitle = "")
		{
			return PlayZone.GlobalPlayZone.ShowBaodianGuideWindow(errType, callback, customLinkIDs, customTitle, false);
		}

		public static void ShowGoodsGuideForGoodsTips(int goodsID, DPSelectedItemEventHandler callback = null)
		{
			byte b = 0;
			int[] goodsObtainIdArray = ConfigGoodsObtain.GetGoodsObtainIdArray(goodsID);
			if (goodsObtainIdArray != null && 0 < goodsObtainIdArray.Length)
			{
				PlayZone.GlobalPlayZone.ShowBaodianGuideWindow(goodsID, goodsObtainIdArray, null, false);
				b = 1;
			}
			if (b == 0)
			{
				string goodsObtainNameByGoodsID = ConfigGoodsObtain.GetGoodsObtainNameByGoodsID(goodsID);
				if (!string.IsNullOrEmpty(goodsObtainNameByGoodsID))
				{
					Super.HintMainText(goodsObtainNameByGoodsID, 10, 3);
				}
			}
		}

		public static int ShowGoodsGuide(int goodsID, DPSelectedItemEventHandler callback = null)
		{
			if (goodsID == 2000)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedMayajingshi, callback, string.Empty, string.Empty);
			}
			else if (goodsID == 2004)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedChuangzaojingshi, callback, string.Empty, string.Empty);
			}
			else if (goodsID == 2002)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedLinghunjingshi, callback, string.Empty, string.Empty);
			}
			else if (goodsID == 2003)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedShengmingjingshi, callback, string.Empty, string.Empty);
			}
			else if (goodsID == 2005)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedShenyoujingshi, callback, string.Empty, string.Empty);
			}
			else if (goodsID == 2001)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZhufujingshi, callback, string.Empty, string.Empty);
			}
			else if (goodsID == 2017)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedHuozhong, callback, string.Empty, string.Empty);
			}
			else if (goodsID == 2016)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedYumao, callback, string.Empty, string.Empty);
			}
			else if (goodsID == 2031)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZaizaojingshi, callback, string.Empty, string.Empty);
			}
			else if (goodsID == 2018)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZhuLing, callback, string.Empty, string.Empty);
			}
			else if (goodsID == 2100)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedMoFaShuiJing, callback, string.Empty, string.Empty);
			}
			else if (goodsID == 2101)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedMoFaJingYanShu, callback, string.Empty, string.Empty);
			}
			else if (goodsID == 2102)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedMoFaSaoZhou, callback, string.Empty, string.Empty);
			}
			else if (goodsID >= 2070 && goodsID <= 2075)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedShangguSuipian, callback, string.Empty, string.Empty);
			}
			else if (goodsID >= 2076 && goodsID <= 2081)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedYuanguSuipian, callback, string.Empty, string.Empty);
			}
			else if (goodsID == 2034)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJuHunJingShi, callback, string.Empty, string.Empty);
			}
			else
			{
				if (goodsID != 2033)
				{
					return 1;
				}
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJuLingJingShi, callback, string.Empty, string.Empty);
			}
			return 0;
		}

		public static void ShowNoticeWindow(Canvas root)
		{
		}

		public static void SwitchSelfWindowAndMainUI(GameObject obj, bool state)
		{
			if (null == obj)
			{
				return;
			}
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.OnSwitchSelfWindowAndMainUI(obj, state);
			}
			obj.SetActive(state);
			Global.Joystick.gameObject.SetActive(!state);
			HUDTextRoot.go.SetActive(!state);
		}

		public static void LoadGoodsList(string goodsIDs, ObservableCollection ItemCollection)
		{
			ItemCollection.Clear();
			if (string.IsNullOrEmpty(goodsIDs))
			{
				return;
			}
			string[] array = goodsIDs.Split(new char[]
			{
				'@'
			});
			if (array.Length == 1)
			{
				Super.LoadOtherJiangLiGoodsList(goodsIDs, ItemCollection, false);
			}
			else
			{
				Super.LoadOtherJiangLiGoodsList(array[0], ItemCollection, false);
				Super.LoadOtherJiangLiGoodsList(array[1], ItemCollection, true);
			}
		}

		public static void LoadOtherJiangLiGoodsList(string goodsStr, ObservableCollection ItemCollection, bool isOcc = false)
		{
			string text = StringUtil.trim(goodsStr);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			string[] array = text.Split(new char[]
			{
				'|'
			});
			if (array.Length <= 0)
			{
				return;
			}
			int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2.Length == 7)
				{
					if (!isOcc || !MUJieripartChongzhiKingItem.IsTongGuo(array2[0], roleOcc))
					{
						GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[6]), Convert.ToInt32(array2[5]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
						Super.AddGoodsIcon(dummyGoodsDataMu, ItemCollection, false, true);
					}
				}
			}
			ItemCollection.DelayUpdate();
		}

		public static void LoadOtherGoodsList(string goodsStr, ObservableCollection ItemCollection, string _effect)
		{
			string text = StringUtil.trim(goodsStr);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			string[] array = text.Split(new char[]
			{
				'|'
			});
			if (array.Length <= 0)
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[6]), Convert.ToInt32(array2[5]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				if (!string.IsNullOrEmpty(_effect))
				{
					string[] array3 = _effect.Split(new char[]
					{
						'|'
					});
					string[] array4 = array3[i].Split(new char[]
					{
						','
					});
					dummyGoodsDataMu.Endtime = ((array4.Length != 3) ? array4[1] : array4[2]);
				}
				Super.AddGoodsIcon(dummyGoodsDataMu, ItemCollection, false, true);
			}
			ItemCollection.DelayUpdate();
		}

		public static GGoodIcon AddGoodsIcon(GoodsData gd, ObservableCollection ItemCollection, bool grayShow = false, bool ShowTips = true)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
				string backSpriteName = "bagGrid4_bak";
				GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
				icon.Width = 78.0;
				icon.Height = 78.0;
				icon.BackSpriteName0 = backSpriteName;
				icon.TipType = 1;
				icon.ItemCategory = goodsXmlNodeByID.Categoriy;
				icon.ItemCode = gd.GoodsID;
				icon.ItemObject = gd;
				icon.BoxTypes = -1;
				if (!grayShow)
				{
					icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
				}
				else
				{
					icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
				}
				bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
				Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
				ItemCollection.Add(icon);
				icon.gameObject.AddComponent<UIDragPanelContents>();
				if (ShowTips)
				{
					icon.addEventListener("click", delegate(MouseEvent s)
					{
						GGoodIcon ggoodIcon = s.target.SafeGetComponent<GGoodIcon>();
						if (null == ggoodIcon)
						{
							return;
						}
						GoodsData goodsData = icon.ItemObject as GoodsData;
						if (goodsData == null)
						{
							return;
						}
						GoodsData goodsData2 = goodsData.Clone();
						goodsData2.GCount = 1;
						GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData2);
					});
				}
				return icon;
			}
			return null;
		}

		public static void PlayAnim(GameObject anim)
		{
			if (null == anim)
			{
				return;
			}
			anim.gameObject.SetActive(false);
			anim.gameObject.SetActive(true);
		}

		public static void ActiveGameObject(GameObject obj, bool state)
		{
			if (null == obj)
			{
				return;
			}
			obj.gameObject.SetActive(state);
		}

		public static void CloseVerifySecondPasswordWindow()
		{
			if (Super.VerifySecondPasswordWindow != null)
			{
				Super.MainWindowRoot.Children.Remove(Super.VerifySecondPasswordWindow, true);
				Super.CloseChildWindow(Super.MainWindowRoot, Super.VerifySecondPasswordWindow);
				Super.VerifySecondPasswordWindow = null;
				if (Super.VerifySecondPassowrdPart != null)
				{
					Super.VerifySecondPassowrdPart = null;
				}
				if (Global.VerifySuccess != null)
				{
					Global.VerifySuccess = null;
				}
			}
		}

		public static void ShowVerifySecondPasswordWindow(int roleID)
		{
			Super.VerifySecondPasswordWindow = U3DUtils.NEW<GChildWindow>();
			Super.VerifySecondPasswordWindow.ModalType = ChildWindowModalType.TransBak;
			UIEventListener.Get(Super.VerifySecondPasswordWindow.ModalBak).onClick = delegate(GameObject go)
			{
				Super.CloseVerifySecondPasswordWindow();
			};
			Super.InitChildWindow(Super.VerifySecondPasswordWindow, Global.GetLang(string.Empty));
			Super.MainWindowRoot.Children.Add(Super.VerifySecondPasswordWindow);
			Super.VerifySecondPasswordWindow.ChildWindowClose = delegate(object s, EventArgs e)
			{
				Super.CloseVerifySecondPasswordWindow();
				return true;
			};
			Super.VerifySecondPassowrdPart = U3DUtils.NEW<VerifySecondPasswordPart>();
			Super.VerifySecondPassowrdPart.VerifiedRoleID = roleID;
			Super.VerifySecondPasswordWindow.Children.Add(Super.VerifySecondPassowrdPart.gameObject);
		}

		public static void ShowJingLingHuiShouMessageBox(string Content, string[] AwardDescribe, int[] money, DPSelectedItemBoolEventHandler hander, string[] btnStr = null)
		{
			GChildWindow gw = U3DUtils.NEW<GChildWindow>();
			gw.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(gw, Global.GetLang(string.Empty));
			Super.GData.PlayZoneRoot.Children.Add(gw);
			UIEventListener.Get(gw.ModalBak).onClick = delegate(GameObject e)
			{
				Super.MainWindowRoot.Children.Remove(gw, true);
				Super.CloseChildWindow(Super.MainWindowRoot, gw);
			};
			gw.ChildWindowClose = delegate(object x, EventArgs e)
			{
				Super.MainWindowRoot.Children.Remove(gw, true);
				Super.CloseChildWindow(Super.MainWindowRoot, gw);
				return true;
			};
			JingLingHuiShouMessagePart jingLingHuiShouMessagePart = U3DUtils.NEW<JingLingHuiShouMessagePart>();
			jingLingHuiShouMessagePart.Contentlabel = Content;
			if (AwardDescribe.Length == 3)
			{
				jingLingHuiShouMessagePart.AwardDescribe0 = AwardDescribe[0];
				jingLingHuiShouMessagePart.AwardDescribe1 = AwardDescribe[1];
				jingLingHuiShouMessagePart.AwardDescribe2 = AwardDescribe[2];
			}
			if (money.Length == 3)
			{
				jingLingHuiShouMessagePart.Money0 = money[0];
				jingLingHuiShouMessagePart.Money1 = money[1];
				jingLingHuiShouMessagePart.Money2 = money[2];
			}
			if (btnStr != null && btnStr.Length == 2)
			{
				jingLingHuiShouMessagePart.BtnSureStr = btnStr[0];
				jingLingHuiShouMessagePart.BtnCancleStr = btnStr[1];
			}
			jingLingHuiShouMessagePart.MessageBtnHander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (s.ID == 0)
				{
					if (hander != null)
					{
						hander(e, s);
					}
				}
				else if (s.ID == 1)
				{
					if (hander != null)
					{
						hander(e, s);
					}
				}
				else if (s.ID == 2)
				{
				}
				Super.MainWindowRoot.Children.Remove(gw, true);
				Super.CloseChildWindow(Super.MainWindowRoot, gw);
				return true;
			};
			gw.Children.Add(jingLingHuiShouMessagePart.gameObject);
		}

		public static bool CheckArrayHaveValue<T>(T[] array, T value)
		{
			if (array != null && 0 < array.Length)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Equals(value))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static string ClearStringColor(string str)
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(str))
			{
				bool flag = false;
				for (int i = 0; i < str.Length; i++)
				{
					if ("{" == str.get_Chars(i).ToString())
					{
						flag = true;
					}
					if ("}" == str.get_Chars(i).ToString())
					{
						flag = false;
					}
					else if (!flag)
					{
						text += str.get_Chars(i).ToString();
					}
				}
			}
			return text;
		}

		public static List<T> ArrayToList<T>(T[] array)
		{
			List<T> list = new List<T>();
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					list.Add(array[i]);
				}
			}
			return list;
		}

		public static int ToInt(double value, byte type = 0)
		{
			string[] array = value.ToString().Split(new char[]
			{
				'.'
			});
			if (array.Length != 2)
			{
				return (int)value;
			}
			if (0.0 < Math.Abs(Global.SafeConvertToDouble(array[1].ToString())))
			{
				return array[0].SafeToInt32(0) + ((type != 0) ? 0 : 1);
			}
			return array[0].SafeToInt32(0);
		}

		public static void ShowCreateFuRolePanelControl(Canvas root, int nType = 0)
		{
			Global.DangQianHuaZhi = Global.Data.SysSetting.GraphicsQuality;
			Global.Data.SysSetting.GraphicsQuality = false;
			if (Global.Data.SysSetting.GraphicsQuality)
			{
				PerformanceCtrl.PerformanceType = PerformanceTypes.HiUsage;
			}
			else
			{
				PerformanceCtrl.PerformanceType = PerformanceTypes.LowUsage;
			}
			GameObject gameObject = GameObject.Find("Leader");
			if (gameObject)
			{
				gameObject.GetComponent<CameraController>().enabled = false;
			}
			Super.ChangeCameraParasName();
			if (null == Super.roleCreator)
			{
				Super.roleCreator = U3DUtils.NEW<RoleCreator>();
				Super.roleCreator.isCreateSecondRole = (nType != 2);
				Super.roleCreator.resetRoleCreatorList();
				Super.roleCreator.SetWindowType(nType);
				RoleCreator roleCreator = Super.roleCreator;
				roleCreator.RolePanelChanged = (EventHandler)Delegate.Combine(roleCreator.RolePanelChanged, delegate(object s2, EventArgs e2)
				{
					Super.CloseRoleCreatorWindow();
				});
			}
			else
			{
				Super.roleCreator.isCreateSecondRole = (nType != 2);
				Super.roleCreator.resetRoleCreatorList();
				Super.roleCreator.Show3DObjects();
			}
			Super.roleCreator.Visibility = true;
			root.Children.Add(Super.roleCreator);
			if (Super.MainGameMgr)
			{
				Super.MainGameMgr.gameObject.SetActive(false);
			}
			Global.IsCanMove = false;
			NGUITools.SetActive(Global.Joystick.gameObject, false);
			HUDTextRoot.go.SetActive(false);
			LayerCullDistanceslMgr.SetCameraLayerDistance(Global.MainCamera, 1000f);
		}

		public static void CloseRoleCreatorWindow()
		{
			if (null != Super.roleCreator)
			{
				Super.roleCreator.transform.parent = null;
				Object.Destroy(Super.roleCreator.gameObject);
				Super.roleCreator = null;
			}
			if (Super.MainGameMgr)
			{
				Super.MainGameMgr.gameObject.SetActive(true);
			}
			GameObject gameObject = GameObject.Find("Leader");
			if (gameObject)
			{
				gameObject.GetComponent<CameraController>().enabled = true;
			}
			GameObject gameObject2 = GameObject.Find("Scene");
			if (gameObject2)
			{
				Object.Destroy(gameObject2);
			}
			Super.ResumeBackUpMainScene();
			Global.IsCanMove = true;
			Global.MainCamera.transform.localPosition = Vector3.zero;
			Global.MainCamera.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
			Global.MainCamera.far = 35f;
			Global.MainCamera.fieldOfView = 30f;
			RenderSettings.ambientLight = new Color(1f, 1f, 1f);
			Global.Data.SysSetting.GraphicsQuality = Global.DangQianHuaZhi;
			if (Global.Data.SysSetting.GraphicsQuality)
			{
				PerformanceCtrl.PerformanceType = PerformanceTypes.HiUsage;
			}
			else
			{
				PerformanceCtrl.PerformanceType = PerformanceTypes.LowUsage;
			}
			Super.DestoryCameraParas();
			Super.ReChangeCameraParasName();
			Super.CopyCameraParmas();
			NGUITools.SetActive(Global.Joystick.gameObject, true);
			HUDTextRoot.go.SetActive(true);
		}

		private static void CopyCameraParmas()
		{
			Global.MainCamera.backgroundColor = Color.black;
			Global.MainCamera.farClipPlane = 50f;
			LayerCullDistanceslMgr.SetCameraLayerDistance(Global.MainCamera, 50f);
			if (Global.GetMapType(Global.Data.roleData.MapCode) != MapTypes.Normal)
			{
				Global.MainCamera.farClipPlane = 100f;
				LayerCullDistanceslMgr.SetCameraLayerDistance(Global.MainCamera, 100f);
			}
			PerformanceCtrl.PerformanceType = PerformanceCtrl.PerformanceType;
			MUDebug.LogError<string>(new string[]
			{
				"性能画面优化 = 3"
			});
			GameObject gameObject = GameObject.Find("CameraParams");
			if (gameObject != null)
			{
				Camera component = gameObject.GetComponent<Camera>();
				float num = 50f;
				string loadedLevelName = Application.loadedLevelName;
				if (loadedLevelName != null)
				{
					if (Super.<>f__switch$mapD == null)
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
						Super.<>f__switch$mapD = dictionary;
					}
					int num2;
					if (Super.<>f__switch$mapD.TryGetValue(loadedLevelName, ref num2))
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

		public static void BackUpMainScene()
		{
			GameObject gameObject = GameObject.Find("Scene");
			while (gameObject != null)
			{
				gameObject.name = "Scene_map_bak";
				gameObject = GameObject.Find("Scene");
			}
		}

		public static void ResumeBackUpMainScene()
		{
			GameObject gameObject = GameObject.Find("Scene_map_bak");
			while (gameObject != null)
			{
				gameObject.name = "Scene";
				gameObject = GameObject.Find("Scene_map_bak");
			}
		}

		public static void DestoryCameraParas()
		{
			GameObject gameObject = GameObject.Find("CameraParams");
			if (gameObject != null)
			{
				NGUITools.Destroy(gameObject);
			}
		}

		public static void ChangeCameraParasName()
		{
			GameObject gameObject = GameObject.Find("CameraParams");
			if (gameObject != null)
			{
				gameObject.name = "CameraParams_oldMap";
			}
		}

		public static void ReChangeCameraParasName()
		{
			GameObject gameObject = GameObject.Find("CameraParams_oldMap");
			if (gameObject != null)
			{
				gameObject.name = "CameraParams";
			}
		}

		public static bool ShowGoodRedTip(GoodsData data)
		{
			if (Super.CanUseGoodIDs == null || Super.CanUseGoodIDs.Length <= 0)
			{
				Super.CanUseGoodIDs = ConfigSystemParam.GetSystemParamIntArrayByName("UseGoodsID", ',');
			}
			if (data.Using == 1 || Super.CanUseGoodIDs == null || Super.CanUseGoodIDs.Length <= 0 || !Enumerable.Contains<int>(Super.CanUseGoodIDs, data.GoodsID))
			{
				return false;
			}
			if (!Global.CanUseGoods(data.GoodsID, false, true))
			{
				return false;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
			bool result = true;
			if (Global.Data != null && Global.Data.roleData != null && Global.Data.roleData.GoodsLimitDataList != null)
			{
				for (int i = 0; i < Global.Data.roleData.GoodsLimitDataList.Count; i++)
				{
					GoodsLimitData goodsLimitData = Global.Data.roleData.GoodsLimitDataList[i];
					if (goodsLimitData.GoodsID == goodsXmlNodeByID.ID)
					{
						result = (goodsLimitData.UsedNum < goodsXmlNodeByID.DayLimit);
						break;
					}
				}
			}
			return result;
		}

		public static int GetHorseQuality(GoodsData gd)
		{
			if (gd != null)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
				if (goodsXmlNodeByID != null)
				{
					if (40 > goodsXmlNodeByID.Categoriy || 45 < goodsXmlNodeByID.Categoriy)
					{
						return goodsXmlNodeByID.ItemQuality;
					}
					int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(gd);
					if (6 <= zhuoyueAttributeCount)
					{
						return 4;
					}
					if (zhuoyueAttributeCount == 5)
					{
						return 3;
					}
					if (3 <= zhuoyueAttributeCount && 5 > zhuoyueAttributeCount)
					{
						return 2;
					}
					if (0 < zhuoyueAttributeCount && 3 > zhuoyueAttributeCount)
					{
						return 1;
					}
					return 0;
				}
			}
			return 0;
		}

		public static void CloseAllUIWindow()
		{
			for (int i = WindowManage.WindowList.Count - 1; i >= 0; i--)
			{
				if (null != WindowManage.WindowList[i].WindowObj && "GChildWindow(Clone)".Equals(WindowManage.WindowList[i].WindowObj.name))
				{
					GChildWindow component = WindowManage.WindowList[i].WindowObj.GetComponent<GChildWindow>();
					if (null != component && null != component.parent)
					{
						Super.CloseChildWindow(component.parent, component);
					}
				}
			}
		}

		private static int GetUPDateSize(List<XElement> needUpdateZIPList)
		{
			int num = 0;
			int num2 = 10485760;
			int num3 = 0;
			float num4 = 0f;
			if (needUpdateZIPList != null && needUpdateZIPList.Count > 0)
			{
				int count = needUpdateZIPList.Count;
				for (int i = 0; i < count; i++)
				{
					XElement xelement = needUpdateZIPList[i];
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "FileSize");
					if (xelementAttributeInt > 0)
					{
						num4 += (float)xelementAttributeInt;
					}
				}
			}
			double num5 = (double)num4;
			MUDebug.Log<string>(new string[]
			{
				"NeedDownloadTotalSize:" + num5
			});
			if (num5 >= (double)num)
			{
				int num6 = (int)(num5 / (double)num2);
				if (num5 % (double)num2 > (double)num2 * 0.5)
				{
					num6++;
				}
				num3 = num6;
				if (num3 == 0)
				{
					num3 = 1;
				}
			}
			return num3;
		}

		public static void ShowUpDateZIPTip(Canvas root, string url, List<XElement> needUpdateZIPList, List<string> needDeleteZIPList)
		{
			MUDebug.Log<string>(new string[]
			{
				"YN_Debug：enter_ShowUpDateZIPTip"
			});
			if (needUpdateZIPList != null && needUpdateZIPList.Count > 0)
			{
				MUDebug.Log<string>(new string[]
				{
					string.Format("YN_Debug:小包需要下载{0}0MB资源", Super.GetUPDateSize(needUpdateZIPList))
				});
				string hintText = string.Format(Global.GetLang("建议您在wifi环境下载，本次需要完整下载{0}0MB资源，是否继续?"), Super.GetUPDateSize(needUpdateZIPList));
				MallMessageBoxPart messageBoxPart = U3DUtils.NEW<MallMessageBoxPart>();
				messageBoxPart.HintTitle = Global.GetLang("提示");
				messageBoxPart.HintText = hintText;
				messageBoxPart.HintText_Labe2.gameObject.SetActive(false);
				messageBoxPart.ButtonClick = delegate(object s, EventArgs e)
				{
					if (messageBoxPart.MyMessageBoxPartReturn == 0)
					{
						MUDebug.Log<string>(new string[]
						{
							"YN_Debug：enter_ShowUpDateZIPTip_if_ButtonClick"
						});
						Super.ShowUpdateGameForZIPUpdate(MainGame._current.Stage, url, needUpdateZIPList, needDeleteZIPList);
					}
					else
					{
						MUDebug.Log<string>(new string[]
						{
							"退出游戏玩家不愿意下载资源"
						});
						Application.Quit();
					}
				};
				MainGame._current.Stage.Children.Add(messageBoxPart);
				messageBoxPart.transform.localPosition = new Vector3(0f, 0f, -12f);
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"YN_Debug:没有资源更新：enter_ShowUpDateZIPTip_if_else"
				});
				Super.ShowUpdateGameForZIPUpdate(MainGame._current.Stage, url, needUpdateZIPList, needDeleteZIPList);
			}
		}

		public static void ShowUpDateCDNTip(Canvas root, int localAppVerCode, int remoteAppVerCode, string url, byte[] remoteVersionBytes, List<XElement> needUpdateList, List<string> needDeleteList)
		{
			MUDebug.Log<string>(new string[]
			{
				"YN_Debug:enter_ShowUpDateCDNTip"
			});
			if (needUpdateList != null && needUpdateList.Count > 0)
			{
				MUDebug.Log<string>(new string[]
				{
					string.Format("YN_Debug:enter_ShowUpDateCDNTip_if：本次CDN需要下载{0}0MB资源", Super.GetUPDateSize(needUpdateList))
				});
				string hintText = string.Format(Global.GetLang("本次需要下载{0}0MB少量增量资源，是否继续?"), Super.GetUPDateSize(needUpdateList));
				MallMessageBoxPart messageBoxPart = U3DUtils.NEW<MallMessageBoxPart>();
				messageBoxPart.HintTitle = Global.GetLang("提示");
				messageBoxPart.HintText = hintText;
				messageBoxPart.HintText_Labe2.gameObject.SetActive(false);
				messageBoxPart.ButtonClick = delegate(object s, EventArgs e)
				{
					if (messageBoxPart.MyMessageBoxPartReturn == 0)
					{
						MUDebug.Log<string>(new string[]
						{
							"YN_Debug：enter_ShowUpDateCDNTip_if_ButtonClick"
						});
						Super.ShowUpdateGameForUpdate(MainGame._current.Stage, localAppVerCode, remoteAppVerCode, url, remoteVersionBytes, needUpdateList, needDeleteList);
					}
					else
					{
						MUDebug.Log<string>(new string[]
						{
							"退出游戏玩家不愿意下载资源2"
						});
						Application.Quit();
					}
				};
				MainGame._current.Stage.Children.Add(messageBoxPart);
				messageBoxPart.transform.localPosition = new Vector3(0f, 0f, -12f);
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"YN_Debug:没有资源更新：enter_ShowUpDateCDNTip_if_else"
				});
				Super.ShowUpdateGameForUpdate(MainGame._current.Stage, localAppVerCode, remoteAppVerCode, url, remoteVersionBytes, needUpdateList, needDeleteList);
			}
		}

		public static SuperData GData = null;

		public static Canvas MainWindowRoot = null;

		public static GameManager MainGameMgr = null;

		public static bool ConnectToGameServerFailed = false;

		public static Dictionary<int, Queue<HintTextdata>> goodsHintDict = new Dictionary<int, Queue<HintTextdata>>();

		public static string[] AutoSystemChatItemsArray = null;

		public static Dictionary<int, int> goodDBIdDict = new Dictionary<int, int>();

		public static int[] MessageBoxIsHint = new int[15];

		private static RoleManager roleManager = null;

		public static RoleCreator roleCreator;

		private static GChildWindow VerifySecondPasswordWindow = null;

		private static VerifySecondPasswordPart VerifySecondPassowrdPart = null;

		public static GameObject ModalLayer = null;

		public static MyMessageBoxExPart m_MessageBoxPart = null;

		public static GameObject DialogLayer = null;

		public static GameObject NetWaiting = null;

		public static bool IsClickedMessageBox = false;

		private static CheckingUpdateGame CheckingGame = null;

		public static UpdateGame UpdateGameInstance = null;

		public static PlatformUserLogin platformLogin = null;

		public static TencentLogin tencentLogin = null;

		private static UserLogin UserLoginInstance;

		public static LoadingMap CurrentLoadingMap = null;

		private static float mScreenWidth = 0f;

		private static float mScreenHeight = 0f;

		private static Canvas ChangingMapWindow = null;

		private static string RedColor = "ff0000";

		private static string args0 = "#0";

		private static string args1 = "#1";

		private static double JugeTeamUIItemCount = 0.0;

		private static Dictionary<int, int> SystemNaviDict = new Dictionary<int, int>();

		private static UIAtlas ZhuoYueFlowLightAtlasInstance = null;

		private static UIAtlas ZhuoYueFlowLightChengAtlasInstance = null;

		private static Dictionary<string, BitmapData> _NetImagesDict = new Dictionary<string, BitmapData>();

		private static Dictionary<string, List<GIcon>> mallWaitingDownloaderDict = new Dictionary<string, List<GIcon>>();

		private static Dictionary<int, bool> _SystemWizardDict = new Dictionary<int, bool>();

		public static CheatMgr CheatManager = new CheatMgr();

		public static int[] CanUseGoodIDs = null;

		public static MallMessageBoxPart m_MallMessageBoxPart = null;
	}
}
