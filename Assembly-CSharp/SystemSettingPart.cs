using System;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using TianMa;
using Umeng;
using UnityEngine;

public class SystemSettingPart : UserControl
{
	public SystemSettingPart()
	{
		this.thisCtrl = this;
	}

	private void InitTextInPrefabs()
	{
		if (Context.IsHaiwai)
		{
			NGUITools.SetActive(this.btnPPCenter.gameObject, false);
			this.m_BtnQuitGame.transform.localPosition = new Vector3(-1f, -108f, 0f);
			NGUITools.SetActive(this.m_BtnCheckNet.gameObject, false);
			this.m_PingBiTeXiao.gameObject.transform.localPosition = this.m_ChkJiaoYou.gameObject.transform.localPosition;
			NGUITools.SetActive(this.m_ChkJiaoYou.gameObject, false);
		}
		this.m_GTab.TabBtns[0].Text = Global.GetLang("退出");
		this.m_GTab.TabBtns[1].Text = Global.GetLang("音乐");
		this.m_GTab.TabBtns[2].Text = Global.GetLang("游戏");
		this.m_GTab.TabBtns[3].Text = Global.GetLang("画质");
		this.m_BtnDengLu.Text = Global.GetLang("返回登录界面");
		this.m_BtnJueSe.Text = Global.GetLang("返回选择角色");
		this.m_BtnLieBiao.Text = Global.GetLang("返回服务器列表");
		this.btnPPCenter.Text = Global.GetLang("用户中心");
		this.m_BtnQuitGame.Text = Global.GetLang("退出游戏");
		this.m_BtnCheckNet.Text = Global.GetLang("检测网络");
		this.m_ChkMusic.Text = Global.GetLang("关闭背景音乐");
		this.m_ChkSound.Text = Global.GetLang("关闭技能音效");
		this.m_BtnQuitGame2.Text = Global.GetLang("退出游戏");
		this.m_RadioChkHuaZhi.Text = Global.GetLang("画质优先");
		this.m_RadioChkLiuChang.Text = Global.GetLang("流畅优先");
		this.m_ChkScreenLockSize.Text = Global.GetLang("固定分辨率");
		this.m_LabelScreenLockSize.text = Global.GetLang("固定分辨率开启后可大幅提高游戏流畅度");
		this.m_ChkDuiWu.Text = Global.GetLang("拒绝组队邀请");
		this.m_ChkJiaoYi.Text = Global.GetLang("拒绝交易申请");
		this.m_ChkSiLiao.Text = Global.GetLang("屏蔽私聊功能");
		this.m_ChkJiaoYou.Text = Global.GetLang("拒绝好友申请");
		this.m_PingBiTeXiao.Text = Global.GetLang("屏蔽他人特效");
		this.m_PingBiWanJia.Text = Global.GetLang("屏蔽其他玩家");
		this.m_ChkFashion.Text = Global.GetLang("屏蔽时装");
		this.PingBiLabTitle.text = Global.GetLang("屏蔽其他玩家");
		this.m_NewPingBiWanJia._Lable.text = Global.GetLang("屏蔽其他玩家所有形象");
		this.m_PingBiWanJiaChiBang._Lable.text = Global.GetLang("仅屏蔽其他玩家翅膀");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitContrlMsg();
		this.m_ChkScreenLockSize.gameObject.SetActive(true);
		this.m_LabelScreenLockSize.gameObject.SetActive(true);
		if (Context.IsHaiwai)
		{
			this.m_ChkScreenLockSize.gameObject.SetActive(false);
			this.m_LabelScreenLockSize.gameObject.SetActive(false);
		}
	}

	private void InitContrlMsg()
	{
		if (null != this.m_BtnDengLu)
		{
			this.m_BtnDengLu.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				PlatSDKMgr._isEnterGame = false;
				Global.MainCamera.backgroundColor = Color.black;
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 101
				});
			};
		}
		if (null != this.m_BtnQuitGame2)
		{
			this.m_BtnQuitGame2.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 105
				});
			};
		}
		if (null != this.m_BtnJueSe)
		{
			this.m_BtnJueSe.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				PlatSDKMgr._isEnterGame = false;
				Global.MainCamera.backgroundColor = Color.black;
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 103
				});
			};
		}
		if (null != this.m_BtnLieBiao)
		{
			this.m_BtnLieBiao.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				PlatSDKMgr._isEnterGame = false;
				Global.MainCamera.backgroundColor = Color.black;
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 104
				});
			};
		}
		if (null != this.m_BtnQuitGame)
		{
			this.m_BtnQuitGame.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 105
				});
			};
		}
		if (null != this.m_Btn3ClosePanle)
		{
			this.m_Btn3ClosePanle.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 106
				});
			};
		}
		NGUITools.SetActive(this.m_BtnCheckNet, false);
		if (null != this.m_BtnCheckNet)
		{
			this.m_BtnCheckNet.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.m_BtnCheckNet.isEnabled = false;
				this.m_BtnCheckNet.Text = Global.GetLang("检测中...");
				TraceUtils.TraceRoute(Super.GetXapParamByName("serverip", "192.168.0.206"), new ICMP_CALLBACK(this.IcmpCallback), new ICMP_OVER_CALLBACK(this.IcmpOverCallback), 30, 3, 200);
			};
		}
		if (null != this.m_ChkMusic)
		{
			if (Global.Data.SysSetting.CloseGameMusic)
			{
				this.m_ChkMusic.Check = true;
			}
			this.m_ChkMusic.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.DPSelectedItem(this.m_ChkMusic, new DPSelectedItemEventArgs
				{
					ID = 201
				});
			};
		}
		if (null != this.m_ChkSound)
		{
			if (Global.Data.SysSetting.CloseGameAudio)
			{
				this.m_ChkSound.Check = true;
			}
			this.m_ChkSound.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.DPSelectedItem(this.m_ChkSound, new DPSelectedItemEventArgs
				{
					ID = 202
				});
			};
		}
		if (null != this.m_ChkDuiWu)
		{
			if (!Global.Data.SysSetting.AutoAcceptTeamApply)
			{
				this.m_ChkDuiWu.Check = true;
			}
			this.m_ChkDuiWu.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.DPSelectedItem(this.m_ChkDuiWu, new DPSelectedItemEventArgs
				{
					ID = 203
				});
			};
		}
		if (null != this.m_ChkJiaoYi)
		{
			if (Global.Data.SysSetting.RefuseExchangeRequest)
			{
				this.m_ChkJiaoYi.Check = true;
			}
			this.m_ChkJiaoYi.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.DPSelectedItem(this.m_ChkJiaoYi, new DPSelectedItemEventArgs
				{
					ID = 204
				});
			};
		}
		if (null != this.m_ChkSiLiao)
		{
			if (Global.Data.SysSetting.RefusePrivateChat)
			{
				this.m_ChkSiLiao.Check = true;
			}
			this.m_ChkSiLiao.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.DPSelectedItem(this.m_ChkSiLiao, new DPSelectedItemEventArgs
				{
					ID = 205
				});
			};
		}
		if (null != this.m_ChkJiaoYou)
		{
			if (Global.Data.SysSetting.HideChatPopupWin)
			{
				this.m_ChkJiaoYou.Check = true;
			}
			this.m_ChkJiaoYou.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.DPSelectedItem(this.m_ChkJiaoYou, new DPSelectedItemEventArgs
				{
					ID = 206
				});
			};
		}
		if (null != this.m_PingBiWanJia)
		{
			if (Global.Data.SysSetting.HideOtherRoles)
			{
				this.m_PingBiWanJia.Check = true;
			}
			this.m_PingBiWanJia.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.DPSelectedItem(this.m_PingBiWanJia, new DPSelectedItemEventArgs
				{
					ID = 207
				});
			};
		}
		if (null != this.m_NewPingBiWanJia)
		{
			this.m_NewPingBiWanJia.Check = Global.Data.SysSetting.HideOtherRolesStatus;
			this.m_NewPingBiWanJia.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				if (Global.Data.SysSetting.HideOtherRolesStatus)
				{
					this.m_NewPingBiWanJia.Check = true;
					return;
				}
				if (this.m_NewPingBiWanJia.Check)
				{
					this.m_PingBiWanJiaChiBang.Check = false;
					Global.Data.SysSetting.HideOtherRolesStatus = false;
					this.DPSelectedItem(this.m_NewPingBiWanJia, new DPSelectedItemEventArgs
					{
						ID = 305
					});
				}
			};
		}
		if (null != this.m_PingBiWanJiaChiBang)
		{
			this.m_PingBiWanJiaChiBang.Check = Global.Data.SysSetting.HideOtherRolesChiBangStatus;
			this.m_PingBiWanJiaChiBang.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				if (Global.Data.SysSetting.HideOtherRolesChiBangStatus)
				{
					this.m_PingBiWanJiaChiBang.Check = true;
					return;
				}
				if (this.m_PingBiWanJiaChiBang.Check)
				{
					this.m_NewPingBiWanJia.Check = false;
					Global.Data.SysSetting.HideOtherRolesChiBangStatus = false;
					this.DPSelectedItem(this.m_PingBiWanJiaChiBang, new DPSelectedItemEventArgs
					{
						ID = 306
					});
				}
			};
		}
		if (null != this.m_PingBiTeXiao)
		{
			if (Global.Data.SysSetting.HideGameEffect)
			{
				this.m_PingBiTeXiao.Check = true;
			}
			this.m_PingBiTeXiao.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.DPSelectedItem(this.m_PingBiTeXiao, new DPSelectedItemEventArgs
				{
					ID = 208
				});
			};
		}
		if (Global.Data.SysSetting.GraphicsQuality)
		{
			if (null != this.m_RadioChkHuaZhi)
			{
				this.m_RadioChkHuaZhi.Check = true;
			}
			if (null != this.m_RadioChkLiuChang)
			{
				this.m_RadioChkLiuChang.Check = false;
			}
		}
		else
		{
			if (null != this.m_RadioChkHuaZhi)
			{
				this.m_RadioChkHuaZhi.Check = false;
			}
			if (null != this.m_RadioChkLiuChang)
			{
				this.m_RadioChkLiuChang.Check = true;
			}
		}
		if (null != this.m_RadioChkHuaZhi)
		{
			this.m_RadioChkHuaZhi.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.DPSelectedItem(this.m_RadioChkHuaZhi, new DPSelectedItemEventArgs
				{
					ID = 301
				});
			};
		}
		if (null != this.m_RadioChkLiuChang)
		{
			this.m_RadioChkLiuChang.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.DPSelectedItem(this.m_RadioChkLiuChang, new DPSelectedItemEventArgs
				{
					ID = 302
				});
			};
		}
		if (null != this.m_ChkScreenLockSize)
		{
			if (Global.Data.SysSetting.ScreenLockSize)
			{
				this.m_ChkScreenLockSize.Check = true;
			}
			else
			{
				this.m_ChkScreenLockSize.Check = false;
			}
			this.m_ChkScreenLockSize.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.DPSelectedItem(this.m_ChkScreenLockSize, new DPSelectedItemEventArgs
				{
					ID = 303
				});
			};
		}
		if (null != this.m_ChkFashion)
		{
		}
		if (null != this.btnPPCenter)
		{
			this.btnPPCenter.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				VideoSystem.GetInstance().CloseVideoView();
				PlatSDKMgr.ShowUserCenter();
			};
		}
		this.m_GTab.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (this.m_nSelectIndex == e.ID)
			{
				return false;
			}
			Global.SaveSystemSettings();
			this.m_nSelectIndex = e.ID;
			return true;
		};
		NGUITools.SetActive(this.m_ChkFashion, false);
	}

	public void IcmpCallback(string content)
	{
		if (this.stringBuilder == null)
		{
			this.stringBuilder = new StringBuilder();
		}
		this.stringBuilder.Append(content);
	}

	public override void Update()
	{
		if (this.isCheckOver)
		{
			if (this.checkNetResult)
			{
				Super.HintMainText(Global.GetLang("检测网络结束，检测信息已发送"), 10, 3);
			}
			else
			{
				Super.HintMainText(Global.GetLang("检测网络结束，不能正确连接服务器"), 10, 3);
			}
			Analytics.ShowFB(this.stringBuilder.ToString());
			this.stringBuilder = null;
			this.m_BtnCheckNet.Text = Global.GetLang("检测网络");
			this.m_BtnCheckNet.isEnabled = true;
			this.isCheckOver = false;
		}
	}

	public void IcmpOverCallback(bool ok)
	{
		if (this.stringBuilder == null)
		{
			this.stringBuilder = new StringBuilder();
		}
		this.checkNetResult = ok;
		if (ok)
		{
			this.stringBuilder.Append("=============>success");
		}
		else
		{
			this.stringBuilder.Append("=============>failed");
		}
		this.isCheckOver = true;
	}

	public void SetTabIndex(int nStartIndex = 0)
	{
		if (null != this.m_GTab && this.m_GTab.TabPages.Count >= nStartIndex)
		{
			this.m_GTab.SetActivePage(nStartIndex);
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.tc = U3DUtils.NEW<GTabControl>();
		this.tc.TabItemOrientation = global::Layout.Horizontal;
		this.tc.TabItemWidth = (double)width;
		this.tc.TabItemHeight = 29.0;
		this.tc.TabWidth = 420.0;
		this.tc.TabHeight = 320.0;
		this.tc.BodyLeft = 15.0;
		this.tc.BodyTop = 30.0;
		this.tc.TabItemPos = new Thickness(15.0, 3.0, 0.0, 0.0);
		for (int i = 0; i < this.TabNames.Length; i++)
		{
			this.tc.AddItem(66, 23, 2, "Images/Plate/tab23_normal.png", "Images/Plate/tab23_normal.png", "Images/Plate/tab23_hover.png", "Images/Plate/tab23_hover.png", this.TabNames[i]);
		}
		this.systemSettingTab1 = U3DUtils.NEW<SystemSettingTab1>();
		this.systemSettingTab1.InitPartSize(434, 374);
		this.systemSettingTab2 = U3DUtils.NEW<SystemSettingTab2>();
		this.systemSettingTab2.InitPartSize(434, 374);
		this.tc.SelectionChanged = delegate(object sender, object item)
		{
			GTabControl gtabControl = sender as GTabControl;
			this.CurrentSelectedTab = this.FindTabIndex((item as GIcon).Text);
			int currentSelectedTab = this.CurrentSelectedTab;
			if (currentSelectedTab != 0)
			{
				if (currentSelectedTab == 1)
				{
					base.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/xtsz_bak2.png"), false, 0);
					gtabControl.SetBody(this.systemSettingTab2);
				}
			}
			else
			{
				base.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/xtsz_bak.png"), false, 0);
				gtabControl.SetBody(this.systemSettingTab1);
			}
		};
		this.Container.Children.Add(this.tc);
		Canvas.SetLeft(this.tc, 3);
		Canvas.SetTop(this.tc, 0);
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("保存");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.systemSettingTab1.SaveSystemSettings();
			Global.SaveSystemSettings();
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(gicon, 250);
		Canvas.SetTop(gicon, 338);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 66.0;
		gicon.Height = 21.0;
		gicon.BodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.NewSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 66.0, 21.0, 3.0, 2.0));
		gicon.Text = Global.GetLang("退出");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 0
				});
			}
		};
		Canvas.SetLeft(gicon, 330);
		Canvas.SetTop(gicon, 338);
		this.Container.Children.Add(gicon);
	}

	public void ResetPage(int pageIndex)
	{
		base.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/xtsz_bak.png"), false, 0);
		this.tc.SetBody(this.systemSettingTab1);
		this.tc.SelectItem(new object[]
		{
			0
		});
		this.systemSettingTab1.LoadSystemSettings();
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
		this.systemSettingTab1.CleanUpChildWindows();
		this.systemSettingTab2.CleanUpChildWindows();
	}

	private int FindTabIndex(string text)
	{
		for (int i = 0; i < this.TabNames.Length; i++)
		{
			if (this.TabNames[i] == text)
			{
				return i;
			}
		}
		return -1;
	}

	private SpriteSL thisCtrl;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton m_BtnDengLu;

	public GButton m_BtnQuitGame2;

	public GButton m_BtnJueSe;

	public GButton m_BtnLieBiao;

	public GButton m_BtnQuitGame;

	public GButton m_Btn3ClosePanle;

	public GButton m_BtnCheckNet;

	public GTab m_GTab;

	public GCheckBox m_ChkMusic;

	public GCheckBox m_ChkSound;

	public GCheckBox m_ChkDuiWu;

	public GCheckBox m_ChkJiaoYi;

	public GCheckBox m_ChkSiLiao;

	public GCheckBox m_ChkJiaoYou;

	public GCheckBox m_PingBiTeXiao;

	public GCheckBox m_PingBiWanJia;

	public GCheckBox m_ChkFashion;

	public GCheckBox m_RadioChkHuaZhi;

	public GCheckBox m_RadioChkLiuChang;

	public GCheckBox m_ChkScreenLockSize;

	public UILabel m_LabelScreenLockSize;

	public GameObject m_GameObjBtnArr1;

	public GameObject m_GameObjBtnArr2;

	public GButton btnPPCenter;

	public GCheckBox m_NewPingBiWanJia;

	public GCheckBox m_PingBiWanJiaChiBang;

	private int m_nSelectIndex;

	public UILabel PingBiLabTitle;

	private StringBuilder stringBuilder;

	private bool isCheckOver;

	private bool checkNetResult;

	private string[] TabNames = new string[]
	{
		Global.GetLang("系统设置"),
		Global.GetLang("快捷键"),
		Global.GetLang("系统")
	};

	private GTabControl tc;

	private int CurrentSelectedTab;

	private SystemSettingTab1 systemSettingTab1;

	private SystemSettingTab2 systemSettingTab2;
}
